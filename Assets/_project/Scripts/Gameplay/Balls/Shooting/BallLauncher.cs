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
    private bool _isDualWieldEnabled;
    private bool _useSecondaryNext;
    private bool _isRainbowVisualEnabled;

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

    public event Action<int> Fired;

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _runModifiers = modifiers;
        _ballQueue.SetRunModifiers(modifiers);
    }

    public void SetDualWieldEnabled(bool isEnabled)
    {
        _isDualWieldEnabled = isEnabled;
        _useSecondaryNext = false;

        _ballQueue.SetCapacityMultiplier(isEnabled ? 2 : 1);
    }

    public void SetRainbowVisualEnabled(bool isEnabled)
    {
        _isRainbowVisualEnabled = isEnabled;
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

        bool canUseSecondary = _isDualWieldEnabled && _playerAimer.HasSecondaryFirePoint;

        bool useSecondary = canUseSecondary && _useSecondaryNext;

        Vector3 firePosition = useSecondary ? _playerAimer.SecondaryFirePointPosition : _playerAimer.FirePointPosition;

        Vector3 velocity = GetLaunchVelocity(out BallStats stats);

        bool hasFired = TryLaunchSingle(firePosition, velocity, stats);

        if (hasFired == false)
        {
            return false;
        }

        if (canUseSecondary)
        {
            _useSecondaryNext = !_useSecondaryNext;
        }

        int revolverIndex = useSecondary ? 1 : 0;
        Fired?.Invoke(revolverIndex);

        return true;
    }

    public void SetBallStats(BallStats stats)
    {
        _ballStats = stats;
    }

    public void ResetRun()
    {
        _runModifiers = null;
        _isRainbowVisualEnabled = false;

        SetDualWieldEnabled(false);

        _ballQueue.ResetQueue();
    }

    private bool TryLaunchSingle(Vector3 position, Vector3 velocity, BallStats stats)
    {
        if (_ballQueue.TryGetBall(out Ball ball) == false)
        {
            return false;
        }

        ball.SetRainbowVisual(_isRainbowVisualEnabled);

        if (ball.Launch(position, velocity, stats))
        {
            return true;
        }

        _ballQueue.ReleaseBall(ball);
        return false;
    }
}