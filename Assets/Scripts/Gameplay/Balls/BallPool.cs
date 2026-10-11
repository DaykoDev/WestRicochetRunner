using UnityEngine;

public class BallPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Ball _ballPrefab;
    [SerializeField, Min(1)] private int _initialSize = 3;
    [SerializeField, Min(1)] private int _maxSize = 50;

    private ComponentPool<Ball> _pool;

    private void Awake()
    {
        _pool = new ComponentPool<Ball>(
            _ballPrefab,
            transform,
            _initialSize,
            _maxSize);

        _pool.Prewarm(_initialSize);
    }

    public Ball Get()
    {
        return _pool.Get();
    }

    public void ResetPool()
    {
        _pool?.Reset();
    }
}