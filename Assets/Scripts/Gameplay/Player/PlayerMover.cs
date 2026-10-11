
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _forwardSpeed = 5f;
    [SerializeField, Min(0f)] private float _lateralSmoothness = 15f;

    private float _targetX;
    private bool _isMovementEnabled;

    private void Update()
    {
        if (_isMovementEnabled == false)
        {
            return;
        }

        MoveForward();
        MoveLaterally();
    }

    public void Initialize(float initialX)
    {
        _targetX = initialX;
    }

    public void SetMovementEnabled(bool isEnabled)
    {
        _isMovementEnabled = isEnabled;
    }

    public void SetTargetX(float targetX)
    {
        _targetX = targetX;
    }

    private void MoveForward()
    {
        transform.position += Vector3.forward * (_forwardSpeed * Time.deltaTime);
    }

    private void MoveLaterally()
    {
        Vector3 currentPosition = transform.position;

        float newX = Mathf.Lerp(currentPosition.x, _targetX, _lateralSmoothness * Time.deltaTime);

        transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);
    }
}