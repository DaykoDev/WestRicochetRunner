using UnityEngine;
using WestRicochet.Gameplay.Effects;
using WestRicochet.Infrastructure.Pooling;

namespace WestRicochet.Gameplay.Balls.Shooting
{
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
            _pool.Prewarm(_initialSize);
        }

        public Ball Get()
        {
            return _pool.Get();
        }

        public void Release(Ball ball)
        {
            _pool?.Release(ball);
        }

        public void Reset()
        {
            _pool?.Reset();
        }

        private void OnBallCreated(Ball ball)
        {
            if (ball == null)
            {
                return;
            }

            ball.SetCamera(_camera);
            ball.SetEffectPool(_effectPool);

            if (_ricochetFeedback == null)
            {
                Debug.LogError("BallPool: Cannot register ball because RicochetFeedback is missing.", this);

                return;
            }

            _ricochetFeedback.Register(ball);
        }
    }
}