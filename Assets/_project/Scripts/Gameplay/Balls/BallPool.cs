using UnityEngine;

public class BallPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Ball _ballPrefab;
    [SerializeField, Min(1)] private int _initialSize = 3;
    [SerializeField, Min(1)] private int _maxSize = 50;

    [SerializeField] private Camera _camera;
    [SerializeField] private EffectPool _effectPool;
    [SerializeField] private RicochetFeedback _ricochetFeedback;

    private ComponentPool<Ball> _pool;

    private void Awake()
    {
        _pool = new ComponentPool<Ball>(_ballPrefab, transform, _initialSize, _maxSize, OnBallCreated);
    }

    public Ball Get()
    {
        return _pool.Get();
    }

    public void Release(Ball ball)
    {
        _pool?.Release(ball);
    }

    public void ResetPool()
    {
        _pool?.Reset();
    }

    private void OnBallCreated(Ball ball)
    {
        ball.SetCamera(_camera);
        ball.SetEffectPool(_effectPool);
        _ricochetFeedback.Register(ball);
    }
}