using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    [Serializable]
    private class ObstacleEntry
    {
        [SerializeField] private Obstacle _prefab;
        [SerializeField, Min(1)] private int _minLevel = 1;
        [SerializeField, Min(1)] private int _weight = 1;
        [SerializeField] private bool _allowSafe = true;
        [SerializeField] private bool _allowChallenge = true;
        [SerializeField] private bool _isWide;

        public Obstacle Prefab => _prefab;

        public bool IsAvailable(int level, ObstacleSection section, bool wide)
        {
            if (_prefab == null || level < _minLevel || _weight <= 0 || _isWide != wide)
            {
                return false;
            }

            return section == ObstacleSection.Safe ? _allowSafe : section == ObstacleSection.Challenge && _allowChallenge;
        }

        public int Weight => _weight;
    }

    [Header("Pooling")]
    [SerializeField] private Transform _poolContainer;
    [SerializeField, Min(0)] private int _prewarmPerType = 5;
    [SerializeField, Min(1)] private int _maxPerType = 30;
    [SerializeField] private EffectPool _explosionEffectPool;

    private readonly Dictionary<Obstacle, ComponentPool<Obstacle>> _pools = new();

    [Header("Obstacle Prefabs")]
    [SerializeField] private ObstacleEntry[] _entries;

    [Header("Calculators")]
    [SerializeField] private ObstacleHealthCalculator _healthCalculator;
    [SerializeField] private IncomeCalculator _incomeCalculator;
    [SerializeField] private RunRewardCollector _rewardCollector;

    [SerializeField] private Camera _camera;

    [Header("Cleanup")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField, Min(4)] private int _cleanupBufferSize = 32;

    private readonly List<ObstacleEntry> _candidates = new();
    private Collider[] _cleanupBuffer;

    private void Awake()
    {
        if (_poolContainer == null)
        {
            _poolContainer = transform;
        }

        foreach (ObstacleEntry entry in _entries)
        {
            if (entry.Prefab == null || _pools.ContainsKey(entry.Prefab))
            {
                continue;
            }

            ComponentPool<Obstacle> pool = new ComponentPool<Obstacle>(entry.Prefab, _poolContainer, _prewarmPerType, _maxPerType, OnObstacleCreated);
            pool.Prewarm(_prewarmPerType);
            _pools.Add(entry.Prefab, pool);
        }

        _cleanupBuffer = new Collider[_cleanupBufferSize];
    }

    public Obstacle Create(int level, ObstacleSection section, Vector3 position, Quaternion rotation, Transform parent, float incomeMultiplier, System.Random random, bool wide)
    {
        if (_healthCalculator == null || _incomeCalculator == null)
        {
            Debug.LogError("ObstacleFactory calculators are not assigned.", this);
            return null;
        }

        _candidates.Clear();
        int totalWeight = 0;

        foreach (ObstacleEntry entry in _entries)
        {
            if (entry.IsAvailable(level, section, wide) == false)
            {
                continue;
            }

            _candidates.Add(entry);
            totalWeight += entry.Weight;
        }

        if (totalWeight == 0)
        {
            return null;
        }

        int roll = random.Next(totalWeight);
        ObstacleEntry selected = null;

        foreach (ObstacleEntry entry in _candidates)
        {
            roll -= entry.Weight;

            if (roll < 0)
            {
                selected = entry;
                break;
            }
        }

        if (selected == null)
        {
            return null;
        }

        ComponentPool<Obstacle> pool = _pools[selected.Prefab];
        Obstacle obstacle = pool.Get();
        obstacle.transform.SetParent(parent);
        obstacle.transform.SetPositionAndRotation(position, rotation);
        obstacle.ConfigureForLevel(level, section, incomeMultiplier, _healthCalculator, _incomeCalculator);

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

    public void BeginRun()
    {
        if (_rewardCollector != null)
        {
            _rewardCollector.BeginRun();
        }
    }

    private void OnObstacleCreated(Obstacle obstacle)
    {
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