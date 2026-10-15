using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Damage Settings")]
    [SerializeField, Min(0f)] private float _invulnerabilityDuration = 1f;

    private float _nextDamageTime;

    private void Awake()
    {
        if (_playerHealth == null)
            _playerHealth = GetComponent<PlayerHealth>();

        if (_playerHealth == null)
        {
            Debug.LogError("PlayerHealth is not assigned.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playerHealth.IsDead || Time.time < _nextDamageTime)
            return;

        Obstacle obstacle = other.GetComponentInParent<Obstacle>();

        if (obstacle == null || obstacle.IsDestroyed)
            return;

        _playerHealth.TakeDamage(obstacle.ContactDamage);
        _nextDamageTime = Time.time + _invulnerabilityDuration;
    }
}