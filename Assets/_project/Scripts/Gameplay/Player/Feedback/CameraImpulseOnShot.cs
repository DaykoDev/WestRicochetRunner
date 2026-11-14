using Cinemachine;
using UnityEngine;
using WestRicochet.Gameplay.Balls.Shooting;

namespace WestRicochet.Gameplay.Player.Feedback
{
    public class CameraImpulseOnShot : MonoBehaviour
    {
        [SerializeField] private BallLauncher _ballLauncher;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            if (_ballLauncher == null)
            {
                Debug.LogError("BallLauncher is not assigned.", this);
            }

            if (_impulseSource == null)
            {
                Debug.LogError("Impulse source is not assigned.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_ballLauncher != null)
            {
                _ballLauncher.Fired += OnFired;
            }
        }

        private void OnDisable()
        {
            if (_ballLauncher != null)
            {
                _ballLauncher.Fired -= OnFired;
            }
        }

        private void OnFired(int revolverIndex)
        {
            if (_impulseSource == null)
            {
                return;
            }

            _impulseSource.GenerateImpulse();
        }
    }
}