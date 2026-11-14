using Cinemachine;
using UnityEngine;
using WestRicochet.Gameplay.Player.Damage;

namespace WestRicochet.Gameplay.Player.Feedback
{
    public class CameraImpulseOnDamage : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private int _lastHealth;
        private bool _hasLastHealth;

        private void Awake()
        {
            if (_playerHealth != null && _impulseSource != null)
            {
                return;
            }

            Debug.LogError("CameraImpulseOnDamage: PlayerHealth or ImpulseSource is not assigned.", this);

            enabled = false;
        }

        private void OnEnable()
        {
            _lastHealth = _playerHealth.CurrentHealth;

            if (_playerHealth != null)
            {
                _playerHealth.HealthChanged += OnHealthChanged;
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.HealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            if (_impulseSource == null)
            {
                return;
            }

            if (_hasLastHealth && currentHealth < _lastHealth)
            {
                _impulseSource.GenerateImpulse();
            }

            _lastHealth = currentHealth;
            _hasLastHealth = true;
        }
    }
}