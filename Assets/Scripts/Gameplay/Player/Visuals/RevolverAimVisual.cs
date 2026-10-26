using UnityEngine;

public class RevolverAimVisual : MonoBehaviour
{
    [SerializeField] private PlayerAimer _aimer;
    [SerializeField] private Transform _aimPivot;
    [SerializeField, Min(0f)] private float _rotationSpeed = 720f;

    private void Awake()
    {
        if (_aimer == null)
        {
            Debug.LogError("PlayerAimer is not assigned.", this);
        }

        if (_aimPivot == null)
        {
            Debug.LogError("Aim pivot is not assigned.", this);
        }
    }

    private void LateUpdate()
    {
        if (_aimer == null || _aimPivot == null)
        {
            return;
        }

        Vector3 direction = _aimer.Direction;

        if (direction.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _aimPivot.rotation = Quaternion.RotateTowards(_aimPivot.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}