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

        _trajectoryRenderer.UpdateTrajectory(_playerAimer.FirePointPosition, _playerAimer.Direction, _ballLauncher.CurrentStats);

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
}