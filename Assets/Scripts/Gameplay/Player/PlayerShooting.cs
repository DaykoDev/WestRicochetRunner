using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private BallLauncher _ballLauncher;
    [SerializeField] private AutoShooter _autoShooter;
    [SerializeField] private PlayerAimer _playerAimer;
    [SerializeField] private AimPointVisual _aimPointVisual;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;

    private bool _isControlEnabled;

    private void Start()
    {
        SetControlEnabled(false);
    }

    private void LateUpdate()
    {
        if (_isControlEnabled == false)
        {
            return;
        }

        Vector3 velocity = _ballLauncher.GetLaunchVelocity(out BallStats stats);

        _trajectoryRenderer.UpdateTrajectory(_playerAimer.FirePointPosition, velocity, stats);

        _aimPointVisual.UpdateVisual( _playerAimer.AimPointPosition, _camera.transform);
    }

    public void SetControlEnabled(bool isEnabled)
    {
        _isControlEnabled = isEnabled;

        _trajectoryRenderer.SetVisible(isEnabled);
        _aimPointVisual.SetVisible(isEnabled);

        if (isEnabled)
        {
            _autoShooter.StartShooting();
        }
        else
        {
            _autoShooter.StopShooting();
        }
    }

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _ballLauncher.SetRunModifiers(modifiers);
        _autoShooter.SetRunModifiers(modifiers);
    }

    public void ResetRun()
    {
        SetControlEnabled(false);
        _autoShooter.SetRunModifiers(null);
        _ballLauncher.ResetRun();
    }
}