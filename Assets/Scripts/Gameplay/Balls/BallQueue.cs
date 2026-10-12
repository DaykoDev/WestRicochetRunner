
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] private BallPool _ballPool;
    [SerializeField, Min(1)] private int _capacity = 3;

    private int _availableCount;

    public int AvailableCount => _availableCount;
    public bool HasAvailableBall => _availableCount > 0;
    public int Capacity => _capacity;

    private void Awake()
    {
        _availableCount = _capacity;
    }

    public bool TryGetBall(out Ball ball)
    {
        ball = null;

        if (_availableCount <= 0)
        {
            return false;
        }

        ball = _ballPool.Get();

        if (ball == null)
        {
            return false;
        }

        _availableCount--;

        ball.ReturnedToPool -= OnBallReturnedToPool;
        ball.ReturnedToPool += OnBallReturnedToPool;

        return true;
    }

    public void ReleaseBall(Ball ball)
    {
        _ballPool.Release(ball);
    }

    public void SetCapacity(int newCapacity)
    {
        newCapacity = Mathf.Max(1, newCapacity);

        int difference = newCapacity - _capacity;

        _capacity = newCapacity;

        _availableCount = Mathf.Clamp(_availableCount + difference, 0, _capacity);
    }

    private void OnBallReturnedToPool(Ball ball)
    {
        ball.ReturnedToPool -= OnBallReturnedToPool;

        _availableCount = Mathf.Min(_availableCount + 1, _capacity);
    }
}