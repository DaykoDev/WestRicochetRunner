using System;
using System.Collections.Generic;
using UnityEngine;
using WestRicochet.Gameplay.Obstacles.Core;
using WestRicochet.Gameplay.Rewards.Currency;

namespace WestRicochet.Gameplay.Rewards.Flow
{
    public class RunRewardCollector : MonoBehaviour
    {
        [SerializeField] private Wallet _wallet;
        [SerializeField] private CurrencyDropSpawner _dropSpawner;
        [SerializeField] private LevelEconomySettings _levelEconomySettings;

        private readonly HashSet<Obstacle> _registeredObstacles = new();

        private bool _completionRewardGranted;

        public event Action<int> CollectedThisRunChanged;

        public int CollectedThisRun { get; private set; }

        private void Awake()
        {
            if (_wallet == null)
            {
                Debug.LogError("Wallet is not assigned.", this);
            }

            if (_dropSpawner == null)
            {
                Debug.LogError("CurrencyDropSpawner is not assigned.", this);
            }
        }

        private void OnEnable()
        {
            if (_dropSpawner != null)
            {
                _dropSpawner.Collected += OnDropCollected;
            }
        }

        private void OnDisable()
        {
            if (_dropSpawner != null)
            {
                _dropSpawner.Collected -= OnDropCollected;
            }
        }

        public void BeginRun()
        {
            UnregisterAllObstacles();
            _dropSpawner?.ReleaseAll();

            CollectedThisRun = 0;
            _completionRewardGranted = false;

            CollectedThisRunChanged?.Invoke(CollectedThisRun);
        }

        public void ClearPickups()
        {
            _dropSpawner?.ReleaseAll();
        }

        public void RegisterObstacle(Obstacle obstacle)
        {
            if (obstacle == null || obstacle.IsDestroyed)
            {
                return;
            }

            if (_registeredObstacles.Add(obstacle))
            {
                obstacle.Destroyed += OnObstacleDestroyed;
            }
        }

        public void GrantCompletionReward(int completedLevel)
        {
            if (_completionRewardGranted)
            {
                return;
            }

            _completionRewardGranted = true;

            if (_levelEconomySettings == null)
            {
                Debug.LogError("LevelEconomySettings is not assigned.", this);

                return;
            }

            int reward = _levelEconomySettings.GetCompletionReward(completedLevel);

            AddCollectedCoins(reward);
        }

        private void AddCollectedCoins(int amount)
        {
            if (amount <= 0 || _wallet == null)
            {
                return;
            }

            CollectedThisRun += amount;
            _wallet.Add(amount);

            CollectedThisRunChanged?.Invoke(CollectedThisRun);
        }

        private void OnObstacleDestroyed(Obstacle obstacle)
        {
            obstacle.Destroyed -= OnObstacleDestroyed;
            _registeredObstacles.Remove(obstacle);

            int reward = obstacle.Income;

            if (reward <= 0)
            {
                return;
            }

            if (_dropSpawner == null)
            {
                AddCollectedCoins(reward);
                return;
            }

            _dropSpawner.Spawn(reward, obstacle.transform.position);
        }

        private void OnDropCollected(int amount)
        {
            AddCollectedCoins(amount);
        }

        private void UnregisterAllObstacles()
        {
            foreach (Obstacle obstacle in _registeredObstacles)
            {
                if (obstacle != null)
                {
                    obstacle.Destroyed -= OnObstacleDestroyed;
                }
            }

            _registeredObstacles.Clear();
        }

        private void OnDestroy()
        {
            UnregisterAllObstacles();
        }
    }
}