using System;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour, IDamageable
{
    [Header("Obstacle Settings")]
    [SerializeField, Min(1)] private int _baseHP = 10;
    [SerializeField, Min(0f)] private float _prefabCoinMultiplier = 1f;
    [SerializeField, Min(1)] private int _contactDamage = 1;

    private float _maxHealth;
    private float _currentHealth;
    private int _income;
    private bool _isDestroyed;

    public int ContactDamage => _contactDamage;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public int Income => _income;
    public bool IsDestroyed => _isDestroyed;

    public event Action<float, float> HealthChanged;
    public event Action<Obstacle> Destroyed;

    protected virtual void Awake()
    {
        Initialize(_baseHP, Mathf.RoundToInt(_baseHP * _prefabCoinMultiplier));
    }

    public void ConfigureForLevel(int level, ObstacleSection section, float incomeMultiplier, ObstacleHealthCalculator healthCalculator, IncomeCalculator incomeCalculator)
    {
        if (healthCalculator == null || incomeCalculator == null)
        {
            Debug.LogError("Obstacle calculators are not assigned.", this);
            return;
        }

        int targetHP = healthCalculator.Calculate(_baseHP, level, section);
        int income = incomeCalculator.Calculate(targetHP, incomeMultiplier, _prefabCoinMultiplier);

        Initialize(targetHP, income);
    }

    private void Initialize(float maxHealth, int income)
    {
        _maxHealth = Mathf.Max(1f, maxHealth);
        _currentHealth = _maxHealth;
        _income = Mathf.Max(0, income);
        _isDestroyed = false;

        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (_isDestroyed || damage <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;
        Destroyed?.Invoke(this);
        OnDestroyed();
        Destroy(gameObject);
    }

    protected virtual void OnDestroyed()
    {
    }
}