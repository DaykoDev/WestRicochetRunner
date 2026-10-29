using System.Collections.Generic;
using UnityEngine;

public class RunRewardCollector : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;

    [Header("Currency Drops")]
    [SerializeField] private CurrencyDropSpawner _dropSpawner;

    [Header("Level Completion")]
    [SerializeField] private CompletionRewardCalculator _completionRewardCalculator;

    private readonly HashSet<Obstacle> _registeredObstacles = new();

    private bool _completionRewardGranted;

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

        if (_completionRewardCalculator == null)
        {
            Debug.LogError("CompletionRewardCalculator is not assigned.", this);
            return;
        }

        int reward = _completionRewardCalculator.Calculate(completedLevel);

        GrantReward(reward);
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
            GrantReward(reward);
            return;
        }

        _dropSpawner.Spawn(reward, obstacle.transform.position);
    }

    private void OnDropCollected(int amount)
    {
        GrantReward(amount);
    }

    private void GrantReward(int amount)
    {
        if (amount <= 0 || _wallet == null)
        {
            return;
        }

        _wallet.Add(amount);
        CollectedThisRun += amount;
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