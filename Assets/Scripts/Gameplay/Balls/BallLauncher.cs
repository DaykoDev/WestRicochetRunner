using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallQueue _ballQueue;
    [SerializeField] private PlayerAimer _playerAimer;

    [Header("Ball Settings")]
    [SerializeField] private BallStats _ballStats;

    public BallStats CurrentStats => _ballStats;

    public bool TryLaunch()
    {
        if (_ballQueue.TryGetBall(out Ball ball) == false)
        {
            return false;
        }

        ball.Launch(_playerAimer.FirePointPosition, _playerAimer.Direction, _ballStats);

        return true;
    }

    public void SetBallStats(BallStats stats)
    {
        _ballStats = stats;
    }
}