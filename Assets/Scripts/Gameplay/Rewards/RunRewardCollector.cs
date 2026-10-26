using System.Collections.Generic;
using UnityEngine;

public class RunRewardCollector : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;

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
    }

    public void BeginRun()
    {
        foreach (Obstacle obstacle in _registeredObstacles)
        {
            if (obstacle != null)
            {
                obstacle.Destroyed -= OnObstacleDestroyed;
            }
        }

        _registeredObstacles.Clear();
        CollectedThisRun = 0;
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

        if (reward <= 0 || _wallet == null)
        {
            return;
        }

        _wallet.Add(reward);
        CollectedThisRun += reward;
    }

    private void OnObstacleDestroyed(Obstacle obstacle)
    {
        obstacle.Destroyed -= OnObstacleDestroyed;
        _registeredObstacles.Remove(obstacle);

        int reward = obstacle.Income;

        if (reward <= 0 || _wallet == null)
        {
            return;
        }

        _wallet.Add(reward);
        CollectedThisRun += reward;
    }

    private void OnDestroy()
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
}