using System;
using UnityEngine;
using WestRicochet.Gameplay.Obstacles.Core;

namespace WestRicochet.Gameplay.Obstacles.Spawning
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
}