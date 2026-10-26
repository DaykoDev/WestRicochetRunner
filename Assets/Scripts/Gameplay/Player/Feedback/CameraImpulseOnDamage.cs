using Cinemachine;
using UnityEngine;

public class CameraImpulseOnDamage : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private int _lastHealth;
    private bool _hasLastHealth;

    private void OnEnable()
    {
        _playerHealth.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _playerHealth.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (_hasLastHealth && currentHealth < _lastHealth)
        {
            _impulseSource.GenerateImpulse();
        }

        _lastHealth = currentHealth;
        _hasLastHealth = true;
    }
}