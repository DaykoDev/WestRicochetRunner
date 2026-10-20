using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    private const float RetryInterval = 0.05f;

    [Header("References")]
    [SerializeField] private BallLauncher _ballLauncher;

    [Header("Shooting Settings")]
    [SerializeField, Min(0.01f)] private float _fireInterval = 0.3f;

    private float _shootTimer;
    private bool _isShooting;
    private RunModifiers _runModifiers;

    private void Update()
    {
        if (_isShooting == false)
        {
            return;
        }

        if (_shootTimer > 0f)
        {
            _shootTimer -= Time.deltaTime;
            return;
        }

        float multiplier = _runModifiers != null ? _runModifiers.FireRateMultiplier : 1f;
        float interval = _fireInterval / multiplier;

        _shootTimer = _ballLauncher.TryLaunch() ? interval : RetryInterval;
    }

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _runModifiers = modifiers;
    }

    public void StartShooting()
    {
        _isShooting = true;
        _shootTimer = 0f;
    }

    public void StopShooting()
    {
        _isShooting = false;
        _shootTimer = 0f;
    }
}