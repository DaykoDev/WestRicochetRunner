using UnityEngine;

public class RevolverAimVisual : MonoBehaviour
{
    [SerializeField] private PlayerAimer _aimer;
    [SerializeField] private Transform _visualRoot;
    [SerializeField, Min(0f)] private float _rotationSpeed = 720f;

    private void Awake()
    {
        if (_aimer == null)
        {
            Debug.LogError("PlayerAimer is not assigned.", this);
        }

        if (_visualRoot == null)
        {
            Debug.LogError("Visual root is not assigned.", this);
        }
    }

    private void LateUpdate()
    {
        if (_aimer == null || _visualRoot == null)
        {
            return;
        }

        Vector3 direction = _aimer.Direction;

        if (direction.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _visualRoot.rotation = Quaternion.RotateTowards(_visualRoot.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}