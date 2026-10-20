using UnityEngine;

public class TrackScroller : MonoBehaviour
{
    [SerializeField] private Transform _scrollRoot;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _speed;
    private bool _isScrolling;

    private void Awake()
    {
        if (_scrollRoot == null)
        {
            Debug.LogError("TrackScroller: Scroll Root is not assigned.", this);
            enabled = false;
            return;
        }

        _initialPosition = _scrollRoot.position;
        _initialRotation = _scrollRoot.rotation;
    }

    private void FixedUpdate()
    {
        if (_isScrolling == false)
        {
            return;
        }

        _scrollRoot.position += Vector3.back * (_speed * Time.fixedDeltaTime);
        Physics.SyncTransforms();
    }

    public void StartScrolling(float speed)
    {
        _speed = Mathf.Max(0f, speed);
        _isScrolling = _speed > 0f;
    }

    public void StopScrolling()
    {
        _isScrolling = false;
        _speed = 0f;
    }

    public void ResetPosition()
    {
        StopScrolling();

        _scrollRoot.SetPositionAndRotation(_initialPosition, _initialRotation);
        Physics.SyncTransforms();
    }
}