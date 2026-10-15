using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxHealth = 3;

    private bool _isDead;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => _isDead;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
            Die();
    }

    public void ResetHealth()
    {
        _isDead = false;
        CurrentHealth = _maxHealth;
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;
        Died?.Invoke();
    }
}