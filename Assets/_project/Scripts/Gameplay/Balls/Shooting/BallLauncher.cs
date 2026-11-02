using System;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallQueue _ballQueue;
    [SerializeField] private PlayerAimer _playerAimer;

    [Header("Base Stats")]
    [SerializeField] private BallStats _ballStats;

    private RunModifiers _runModifiers;


    public BallStats CurrentStats
    {
        get
        {
            BallStats stats = _ballStats;

            if (_runModifiers == null)
            {
                return stats;
            }

            stats.Damage += _runModifiers.BallDamageBonus;
            stats.Speed *= _runModifiers.BallSpeedMultiplier;
            stats.MaxBounces += _runModifiers.BounceCountBonus;

            return stats;
        }
    }

    public event Action Fired;

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _runModifiers = modifiers;
        _ballQueue.SetRunModifiers(modifiers);
    }

    public Vector3 GetLaunchVelocity(out BallStats stats)
    {
        stats = CurrentStats;

        if (_playerAimer == null)
        {
            return Vector3.zero;
        }

        return _playerAimer.Direction * stats.Speed;
    }

    public bool TryLaunch()
    {
        if (_ballQueue == null || _playerAimer == null)
        {
            return false;
        }

        if (_ballQueue.TryGetBall(out Ball ball) == false)
        {
            return false;
        }

        Vector3 velocity = GetLaunchVelocity(out BallStats stats);

        if (ball.Launch(_playerAimer.FirePointPosition, velocity, stats) == false)
        {
            _ballQueue.ReleaseBall(ball);
            return false;
        }

        Fired?.Invoke();

        return true;
    }

    public void SetBallStats(BallStats stats)
    {
        _ballStats = stats;
    }

    public void ResetRun()
    {
        _runModifiers = null;
        _ballQueue.ResetQueue();
    }
}