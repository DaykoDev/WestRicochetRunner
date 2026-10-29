using DG.Tweening;
using UnityEngine;

public class PooledEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticle;
    [SerializeField, Min(0.1f)] private float _lifetime = 1.2f;

    private EffectPool _pool;
    private Tween _releaseTween;

    public void SetPool(EffectPool pool)
    {
        _pool = pool;
    }

    public void PlayAt(Vector3 position)
    {
        _releaseTween?.Kill();

        transform.position = position;

        _explosionParticle.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        _explosionParticle.Play(true);

        _releaseTween = DOVirtual.DelayedCall(
                _lifetime,
                Release)
            .SetUpdate(true);
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

        if (_explosionParticle != null)
        {
            _explosionParticle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}