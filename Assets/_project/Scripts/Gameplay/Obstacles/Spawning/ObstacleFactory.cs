using System;
using System.Collections.Generic;
using UnityEngine;
using WestRicochet.Gameplay.Effects;
using WestRicochet.Gameplay.Obstacles.Calculations;
using WestRicochet.Gameplay.Obstacles.Core;
using WestRicochet.Gameplay.Obstacles.Types;
using WestRicochet.Gameplay.Rewards.Flow;
using WestRicochet.Infrastructure.Pooling;

namespace WestRicochet.Gameplay.Obstacles.Spawning
{
    public class ObstacleFactory : MonoBehaviour
    {
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
        [SerializeField] private RunRewardCollector _rewardCollector;

        private readonly Dictionary<Obstacle, ComponentPool<Obstacle>> _pools = new();

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
        }

        public Obstacle Create(
            Obstacle prefab,
            int level,
            ObstacleSection section,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            float healthMultiplier = 1f)
        {
            if (prefab == null)
            {
                Debug.LogError("Obstacle prefab is not assigned.", this);
                return null;
            }

            if (_healthCalculator == null)
            {
                Debug.LogError("ObstacleFactory health calculator is not assigned.", this);
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
                _healthCalculator,
                healthMultiplier);

            if (_rewardCollector != null)
            {
                _rewardCollector.RegisterObstacle(obstacle);
            }

            return obstacle;
        }

        public void ReleaseAll()
        {
            foreach (ComponentPool<Obstacle> pool in _pools.Values)
            {
                pool.Reset();
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
}