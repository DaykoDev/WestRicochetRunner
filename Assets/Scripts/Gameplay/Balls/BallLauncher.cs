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
                return stats;

            stats.Speed *= _runModifiers.BallSpeedMultiplier;
            stats.MaxBounces += _runModifiers.BounceCountBonus;

            return stats;
        }
    }

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _runModifiers = modifiers;
        _ballQueue.SetRunModifiers(modifiers);
    }

    public bool TryLaunch()
    {
        if (_ballQueue == null || _playerAimer == null)
            return false;

        if (!_ballQueue.TryGetBall(out Ball ball))
            return false;

        if (!ball.Launch(_playerAimer.FirePointPosition, _playerAimer.Direction, CurrentStats))
        {
            _ballQueue.ReleaseBall(ball);
            return false;
        }

        return true;
    }

    public void SetBallStats(BallStats stats)
    {
        _ballStats = stats;
    }
}