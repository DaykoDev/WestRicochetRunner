using DG.Tweening;
using UnityEngine;

public sealed class FinishCelebration : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] _fireworks;

    [Header("Finish Line")]
    [SerializeField] private Transform _finishLineVisual;
    [SerializeField, Min(0f)] private float _linePunchDuration = 0.3f;
    [SerializeField, Min(0f)] private float _linePunchScale = 0.08f;

    private Tween _lineTween;
    private Vector3 _initialLineScale;

    private void Awake()
    {
        if (_finishLineVisual != null)
        {
            _initialLineScale = _finishLineVisual.localScale;
        }
    }

    public void Play()
    {
        PlayFireworks();
        PunchFinishLine();
    }

    private void PlayFireworks()
    {
        if (_fireworks == null)
        {
            return;
        }

        foreach (ParticleSystem effect in _fireworks)
        {
            if (effect == null)
            {
                continue;
            }

            effect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            effect.Play(true);
        }
    }

    private void PunchFinishLine()
    {
        if (_finishLineVisual == null)
        {
            return;
        }

        _lineTween?.Kill();
        _finishLineVisual.localScale = _initialLineScale;

        _lineTween = _finishLineVisual
            .DOPunchScale(
                Vector3.one * _linePunchScale,
                _linePunchDuration,
                1,
                0f)
            .OnComplete(() =>
            {
                _finishLineVisual.localScale = _initialLineScale;
                _lineTween = null;
            });
    }

    private void OnDisable()
    {
        _lineTween?.Kill();
        _lineTween = null;

        if (_finishLineVisual != null)
        {
            _finishLineVisual.localScale = _initialLineScale;
        }
    }
}