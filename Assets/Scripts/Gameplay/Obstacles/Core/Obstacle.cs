using System;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour, IDamageable, IPoolable<Obstacle>
{
    [Header("Obstacle Settings")]
    [SerializeField, Min(1)] private int _baseHP = 10;
    [SerializeField, Min(0f)] private float _prefabCoinMultiplier = 1f;
    [SerializeField, Min(1)] private int _contactDamage = 1;

    [Header("Grid")]
    [SerializeField] private Vector2Int _gridSize = Vector2Int.one;

    private int _maxHealth;
    private int _currentHealth;
    private int _income;
    private bool _isDestroyed;

    public int ContactDamage => _contactDamage;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int Income => _income;
    public bool IsDestroyed => _isDestroyed;
    public Vector2Int GridSize => _gridSize;

    public event Action<int, int> HealthChanged;
    public event Action<Obstacle> Destroyed;
    public event Action<Obstacle> Died;
    public event Action<Obstacle> ReturnedToPool;

    public void ConfigureForLevel(
        int level,
        ObstacleSection section,
        float incomeMultiplier,
        ObstacleHealthCalculator healthCalculator,
        IncomeCalculator incomeCalculator)
    {
        if (healthCalculator == null || incomeCalculator == null)
        {
            Debug.LogError("Obstacle calculators are not assigned.", this);
            return;
        }

        int targetHP = healthCalculator.Calculate(
            _baseHP,
            level,
            section);

        int income = incomeCalculator.Calculate(
            targetHP,
            incomeMultiplier,
            _prefabCoinMultiplier);

        Initialize(targetHP, income);
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed || damage <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void ReleaseWithoutReward()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;
        Died?.Invoke(this);
    }

    public void OnReturnedToPool()
    {
        ReturnedToPool?.Invoke(this);
    }

    protected virtual void OnDied()
    {
    }

    private void Initialize(int maxHealth, int income)
    {
        _maxHealth = Mathf.Max(1, maxHealth);
        _currentHealth = _maxHealth;
        _income = Mathf.Max(0, income);
        _isDestroyed = false;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private void Die()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;
        OnDied();
        Destroyed?.Invoke(this);
        Died?.Invoke(this);
    }

    private void OnValidate()
    {
        _gridSize.x = Mathf.Max(1, _gridSize.x);
        _gridSize.y = Mathf.Max(1, _gridSize.y);
    }
}