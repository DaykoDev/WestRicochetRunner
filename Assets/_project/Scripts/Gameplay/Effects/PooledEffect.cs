using DG.Tweening;
using UnityEngine;

namespace WestRicochet.Gameplay.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledEffect : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _lifetime = 1.2f;

        private ParticleSystem _particle;
        private EffectPool _pool;
        private Tween _releaseTween;

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
        }

        public void SetPool(EffectPool pool)
        {
            _pool = pool;
        }

        public void PlayAt(Vector3 position)
        {
            if (_particle == null)
            {
                Debug.LogError("PooledEffect: Particle is missing.", this);
                _pool?.Release(this);

                return;
            }

            _releaseTween?.Kill();

            transform.position = position;

            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            _particle.Play(true);

            _releaseTween = DOVirtual.DelayedCall(_lifetime, Release).SetUpdate(true);
        }

        private void Release()
        {
            _releaseTween = null;
            _pool?.Release(this);
        }

        private void OnDisable()
        {
            _releaseTween?.Kill();
            _releaseTween = null;

            if (_particle != null)
            {
                _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}