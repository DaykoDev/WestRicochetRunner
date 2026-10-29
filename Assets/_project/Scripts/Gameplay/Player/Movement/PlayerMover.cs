using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _forwardSpeed = 5f;
    [SerializeField, Min(0f)] private float _lateralSmoothness = 15f;

    private float _targetX;
    private bool _isMovementEnabled;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    public float ForwardSpeed => _forwardSpeed;

    private void Update()
    {
        if (_isMovementEnabled == false)
        {
            return;
        }

        MoveLaterally();
    }

    public void Initialize(float initialX)
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _targetX = initialX;
    }

    public void ResetToStart()
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
        _targetX = _startPosition.x;
        _isMovementEnabled = false;
    }

    public void SetMovementEnabled(bool isEnabled)
    {
        _isMovementEnabled = isEnabled;
    }

    public void SetTargetX(float targetX)
    {
        _targetX = targetX;
    }

    private void MoveLaterally()
    {
        Vector3 currentPosition = transform.position;

        float damping = 1f - Mathf.Exp(-_lateralSmoothness * Time.deltaTime);
        float newX = Mathf.Lerp(currentPosition.x, _targetX, damping);

        transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);
    }
}