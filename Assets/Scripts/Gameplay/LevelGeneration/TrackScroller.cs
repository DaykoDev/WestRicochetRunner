using System;
using UnityEngine;

public class TrackScroller : MonoBehaviour
{
    [SerializeField] private Transform _scrollRoot;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _speed;
    private bool _isScrolling;

    public float DistanceScrolled { get; private set; }

    public event Action<float> DistanceChanged;

    private void Awake()
    {
        if (_scrollRoot == null)
        {
            Debug.LogError("TrackScroller: scroll root is not assigned.", this);
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

        float delta = _speed * Time.fixedDeltaTime;
        _scrollRoot.position += Vector3.back * delta;

        DistanceScrolled += delta;
        DistanceChanged?.Invoke(DistanceScrolled);
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

        DistanceScrolled = 0f;
        DistanceChanged?.Invoke(DistanceScrolled);
    }
}