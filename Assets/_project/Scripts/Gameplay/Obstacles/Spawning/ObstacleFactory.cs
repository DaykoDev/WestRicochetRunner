using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    [Serializable]
    public sealed class ObstacleEntry
    {
        [SerializeField] private Obstacle _prefab;
        [SerializeField, Min(1)] private int _minLevel = 1;
        [SerializeField, Min(1)] private int _weight = 1;
        [SerializeField] private bool _allowSafe = true;
        [SerializeField] private bool _allowChallenge = true;

        public Obstacle Prefab => _prefab;
        public int Weight => _weight;

        public bool IsAvailable(int level, ObstacleSection section)
        {
            if (_prefab == null || level < _minLevel || _weight <= 0)
            {
                return false;
            }

            return section == ObstacleSection.Safe
                ? _allowSafe
                : section == ObstacleSection.Challenge && _allowChallenge;
        }
    }

    [Header("Pooling")]
    [SerializeField] private Transform _poolContainer;
    [SerializeField, Min(0)] private int _prewarmPerType = 5;
    [SerializeField, Min(1)] private int _maxPerType = 30;
    [SerializeField] private EffectPool _explosionEffectPool;
    [SerializeField] private EffectPool _obstacleDestructionEffectPool;

    [Header("Obstacle Prefabs")]
    [SerializeField] private ObstacleEntry[] _entries;

    [Header("Calculators")]
    [SerializeField] private ObstacleHealthCalculator _healthCalculator;
    [SerializeField] private IncomeCalculator _incomeCalculator;
    [SerializeField] private RunRewardCollector _rewardCollector;

    [Header("Cleanup")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField, Min(4)] private int _cleanupBufferSize = 32;

    private readonly Dictionary<Obstacle, ComponentPool<Obstacle>> _pools = new();
    private Collider[] _cleanupBuffer;

    public IReadOnlyList<ObstacleEntry> Entries => _entries;

    private void Awake()
    {
        if (_poolContainer == null)
        {
            _poolContainer = transform;
        }

        if (_entries == null)
        {
            Debug.LogError("Obstacle entries are not assigned.", this);
            _entries = Array.Empty<ObstacleEntry>();
        }

        foreach (ObstacleEntry entry in _entries)
        {
            if (entry == null ||
                entry.Prefab == null ||
                _pools.ContainsKey(entry.Prefab))
            {
                continue;
            }

            ComponentPool<Obstacle> pool = new ComponentPool<Obstacle>(
                entry.Prefab,
                _poolContainer,
                _prewarmPerType,
                _maxPerType,
                OnObstacleCreated);

            pool.Prewarm(_prewarmPerType);
            _pools.Add(entry.Prefab, pool);
        }

        _cleanupBuffer = new Collider[_cleanupBufferSize];
    }

    public Obstacle Create(
        Obstacle prefab,
        int level,
        ObstacleSection section,
        Vector3 position,
        Quaternion rotation,
        Transform parent,
        float incomeMultiplier)
    {
        if (prefab == null)
        {
            Debug.LogError("Obstacle prefab is not assigned.", this);
            return null;
        }

        if (_healthCalculator == null || _incomeCalculator == null)
        {
            Debug.LogError("ObstacleFactory calculators are not assigned.", this);
            return null;
        }

        if (_pools.TryGetValue(prefab, out ComponentPool<Obstacle> pool) == false)
        {
            Debug.LogError($"No pool registered for {prefab.name}.", this);
            return null;
        }

        Obstacle obstacle = pool.Get();
        obstacle.transform.SetParent(parent);
        obstacle.transform.SetPositionAndRotation(position, rotation);

        obstacle.ConfigureForLevel(
            level,
            section,
            incomeMultiplier,
            _healthCalculator,
            _incomeCalculator);

        if (_rewardCollector != null)
        {
            _rewardCollector.RegisterObstacle(obstacle);
        }

        return obstacle;
    }

    public int ReleaseObstaclesNear(Vector3 center, float radius)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            center,
            radius,
            _cleanupBuffer,
            _obstacleMask,
            QueryTriggerInteraction.Ignore);

        int releasedCount = 0;

        for (int i = 0; i < hitCount; i++)
        {
            Obstacle obstacle = _cleanupBuffer[i].GetComponentInParent<Obstacle>();

            if (obstacle == null || obstacle.IsDestroyed)
            {
                continue;
            }

            obstacle.ReleaseWithoutReward();
            releasedCount++;
        }

        return releasedCount;
    }

    public void ReleaseAll()
    {
        foreach (ComponentPool<Obstacle> pool in _pools.Values)
        {
            pool.Reset();
        }
    }

    public void BeginRun()
    {
        if (_rewardCollector != null)
        {
            _rewardCollector.BeginRun();
        }
    }

    private void OnObstacleCreated(Obstacle obstacle)
    {
        ObstacleVisualFeedback visualFeedback = obstacle.GetComponent<ObstacleVisualFeedback>();

        if (visualFeedback != null)
        {
            visualFeedback.SetDestructionEffectPool(_obstacleDestructionEffectPool);
        }

        if (obstacle is DynamiteBarrel dynamiteBarrel)
        {
            if (_explosionEffectPool == null)
            {
                Debug.LogError("Explosion effect pool is not assigned.", this);

                return;
            }

            dynamiteBarrel.SetExplosionPool(_explosionEffectPool);
        }
    }
}