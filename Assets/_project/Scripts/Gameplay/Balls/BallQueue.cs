using System;
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] private BallPool _ballPool;
    [SerializeField, Min(1)] private int _capacity = 3;

    private int _currentCapacity;
    private int _availableCount;
    private RunModifiers _runModifiers;

    public int Capacity => _currentCapacity;
    public int AvailableCount => _availableCount;
    public bool HasAvailableBall => _availableCount > 0;

    public event Action<int, int> AvailableCountChanged;

    private void Awake()
    {
        _currentCapacity = _capacity;
        _availableCount = _currentCapacity;
    }

    public void SetRunModifiers(RunModifiers modifiers)
    {
        if (_runModifiers != null)
        {
            _runModifiers.Changed -= OnModifiersChanged;
        }

        _runModifiers = modifiers;

        if (_runModifiers != null)
        {
            _runModifiers.Changed += OnModifiersChanged;
        }

        UpdateCapacity();
    }

    public bool TryGetBall(out Ball ball)
    {
        ball = null;

        if (_availableCount <= 0 || _ballPool == null)
        {
            return false;
        }

        ball = _ballPool.Get();

        if (ball == null)
        {
            return false;
        }

        _availableCount--;
        AvailableCountChanged?.Invoke(_availableCount, _currentCapacity);

        ball.ReturnedToPool -= OnBallReturnedToPool;
        ball.ReturnedToPool += OnBallReturnedToPool;

        return true;
    }

    public void ReleaseBall(Ball ball)
    {
        if (ball != null)
        {
            _ballPool.Release(ball);
        }
    }

    public void ResetQueue()
    {
        SetRunModifiers(null);
        _ballPool?.ResetPool();
        _availableCount = _currentCapacity;
        AvailableCountChanged?.Invoke(_availableCount, _currentCapacity);
    }

    public void SetCapacity(int newCapacity)
    {
        newCapacity = Mathf.Max(1, newCapacity);

        int difference = newCapacity - _currentCapacity;
        _currentCapacity = newCapacity;
        _availableCount = Mathf.Clamp(_availableCount + difference, 0, _currentCapacity);
        AvailableCountChanged?.Invoke(_availableCount, _currentCapacity);
    }

    private void UpdateCapacity()
    {
        int bonus = _runModifiers != null ? _runModifiers.BallCountBonus : 0;
        SetCapacity(_capacity + bonus);
    }

    private void OnModifiersChanged()
    {
        UpdateCapacity();
    }

    private void OnBallReturnedToPool(Ball ball)
    {
        ball.ReturnedToPool -= OnBallReturnedToPool;
        _availableCount = Mathf.Min(_availableCount + 1, _currentCapacity);
        AvailableCountChanged?.Invoke(_availableCount, _currentCapacity);
    }

    private void OnDestroy()
    {
        if (_runModifiers != null)
        {
            _runModifiers.Changed -= OnModifiersChanged;
        }
    }
}