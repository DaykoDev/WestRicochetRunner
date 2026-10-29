using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CurrencyDropSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyPickup _pickupPrefab;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _dropRoot;

    [Header("Pool")]
    [SerializeField, Min(0)] private int _initialSize = 20;
    [SerializeField, Min(1)] private int _maxSize = 100;

    [Header("Drop Split")]
    [SerializeField, Min(1)] private int _targetValuePerPickup = 10;
    [SerializeField, Min(1)] private int _maxPickupsPerObstacle = 5;

    private ComponentPool<CurrencyPickup> _pool;

    public Transform DropRoot => _dropRoot;

    public event Action<int> Collected;

    private void Awake()
    {
        if (_dropRoot == null)
        {
            _dropRoot = transform;
        }

        if (_pickupPrefab == null || _player == null)
        {
            Debug.LogError("CurrencyDropSpawner: pickup prefab or player is not assigned.", this);

            return;
        }

        _pool = new ComponentPool<CurrencyPickup>(
            _pickupPrefab,
            transform,
            _initialSize,
            _maxSize);

        _pool.Prewarm(_initialSize);
    }

    public void Spawn(int totalValue, Vector3 position)
    {
        if (totalValue <= 0 || _pool == null)
        {
            return;
        }

        int count = Mathf.CeilToInt(totalValue / (float)_targetValuePerPickup);

        count = Mathf.Clamp(count, 1, _maxPickupsPerObstacle);
        count = Mathf.Min(count, totalValue);

        int baseValue = totalValue / count;
        int remainder = totalValue % count;

        for (int i = 0; i < count; i++)
        {
            int value = baseValue + (i < remainder ? 1 : 0);
            CurrencyPickup pickup = _pool.Get();
            pickup.Launch(this, _player, value, position);
        }
    }

    public void ReleaseAll()
    {
        _pool?.Reset();
    }

    public void ReleaseCollected(CurrencyPickup pickup)
    {
        if (pickup == null || _pool == null)
        {
            return;
        }

        int value = pickup.Value;
        _pool.Release(pickup);
        Collected?.Invoke(value);
    }

    public void ReleaseExpired(CurrencyPickup pickup)
    {
        if (pickup != null)
        {
            _pool?.Release(pickup);
        }
    }

    private void OnValidate()
    {
        _maxSize = Mathf.Max(1, _maxSize);
        _initialSize = Mathf.Clamp(_initialSize, 0, _maxSize);
        _maxPickupsPerObstacle = Mathf.Max(1, _maxPickupsPerObstacle);
    }
}