using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallLauncher _ballLauncher;

    [Header("Shooting Settings")]
    [SerializeField, Min(0.01f)] private float _fireInterval = 0.3f;

    private float _shootTimer;
    private bool _isShooting;

    private void Update()
    {
        if (_isShooting == false)
        {
            return;
        }

        _shootTimer -= Time.deltaTime;

        if (_shootTimer > 0f)
        {
            return;
        }

        if (_ballLauncher.TryLaunch())
        {
            _shootTimer = _fireInterval;
        }
        else
        {
            _shootTimer = 0.05f;
        }
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