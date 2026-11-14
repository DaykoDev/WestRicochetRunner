using UnityEngine;
using WestRicochet.Gameplay.Balls;
using WestRicochet.Gameplay.Balls.Shooting;
using WestRicochet.Gameplay.Run;

namespace WestRicochet.Gameplay.Player.Shooting
{
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
        private bool _isValid;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _isValid = ValidateReferences();

            if (_isValid == false)
            {
                enabled = false;
            }

            SetControlEnabled(false);
        }

        private void LateUpdate()
        {
            if (_isControlEnabled == false)
            {
                return;
            }

            Vector3 velocity = _ballLauncher.GetLaunchVelocity(out BallStats stats);

            _trajectoryRenderer.UpdateTrajectory(_playerAimer.TrajectoryOriginPosition, velocity, stats);

            _aimPointVisual.UpdateVisual(_playerAimer.AimPointPosition, _camera.transform);
        }

        public void SetControlEnabled(bool isEnabled)
        {
            if (_isValid == false)
            {
                return;
            }

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
            if (_isValid == false)
            {
                return;
            }

            _ballLauncher.SetRunModifiers(modifiers);
            _autoShooter.SetRunModifiers(modifiers);
        }

        public void ResetRun()
        {
            if (_isValid == false)
            {
                return;
            }

            SetControlEnabled(false);
            _autoShooter.SetRunModifiers(null);
            _ballLauncher.ResetRun();
        }

        private bool ValidateReferences()
        {
            bool hasReferences =
                _ballLauncher != null &&
                _autoShooter != null &&
                _playerAimer != null &&
                _aimPointVisual != null &&
                _trajectoryRenderer != null &&
                _camera != null;

            if (hasReferences)
            {
                return true;
            }

            Debug.LogError(
                "PlayerShooting: One or more required references are not assigned. " +
                "Check BallLauncher, AutoShooter, PlayerAimer, AimPointVisual, " +
                "TrajectoryRenderer and Camera.",
                this);

            return false;
        }
    }
}