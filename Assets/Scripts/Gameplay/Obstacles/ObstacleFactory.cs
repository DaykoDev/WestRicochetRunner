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

        public Obstacle Prefab => _prefab;

        public bool IsAvailable(int level, ObstacleSection section)
        {
            if (_prefab == null || level < _minLevel || _weight <= 0)
            {
                return false;
            }

            return section == ObstacleSection.Safe ? _allowSafe : section == ObstacleSection.Challenge && _allowChallenge;
        }

        public int Weight => _weight;
    }

    [Header("Obstacle Prefabs")]
    [SerializeField] private ObstacleEntry[] _entries;

    [Header("Calculators")]
    [SerializeField] private ObstacleHealthCalculator _healthCalculator;
    [SerializeField] private IncomeCalculator _incomeCalculator;
    [SerializeField] private RunRewardCollector _rewardCollector;

    private readonly List<ObstacleEntry> _candidates = new();

    public Obstacle Create(int level, ObstacleSection section, Vector3 position, Quaternion rotation, Transform parent, float incomeMultiplier, System.Random random)
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
            if (entry.IsAvailable(level, section) == false)
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

        Obstacle obstacle = Instantiate(selected.Prefab, position, rotation, parent);
        obstacle.ConfigureForLevel(level, section, incomeMultiplier, _healthCalculator, _incomeCalculator);

        if (_rewardCollector != null)
        {
            _rewardCollector.RegisterObstacle(obstacle);
        }

        return obstacle;
    }

    public void BeginRun()
    {
        if (_rewardCollector != null)
        {
            _rewardCollector.BeginRun();
        }
    }
}