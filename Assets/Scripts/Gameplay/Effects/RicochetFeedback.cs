using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class RicochetFeedback : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private EffectPool _sparkPool;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _ricochetClip;
    [SerializeField, Range(0f, 1f)] private float _volume = 0.6f;
    [SerializeField] private Vector2 _pitchRange = new Vector2(0.9f, 1.1f);

    [Header("Camera")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private readonly List<Ball> _balls = new();

    private void Awake()
    {
        if (_sparkPool == null || _audioSource == null || _ricochetClip == null || _impulseSource == null)
        {
            Debug.LogError("RicochetFeedback references are missing.", this);
        }
    }

    public void Register(Ball ball)
    {
        if (ball == null || _balls.Contains(ball))
        {
            return;
        }

        _balls.Add(ball);
        ball.Ricocheted += OnBallRicocheted;
    }

    private void OnDestroy()
    {
        foreach (Ball ball in _balls)
        {
            if (ball != null)
            {
                ball.Ricocheted -= OnBallRicocheted;
            }
        }

        _balls.Clear();
    }

    private void OnBallRicocheted(Vector3 position)
    {
        if (_sparkPool != null)
        {
            _sparkPool.Get().PlayAt(position);
        }

        if (_audioSource != null && _ricochetClip != null)
        {
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            _audioSource.PlayOneShot(_ricochetClip, _volume);
        }

        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }
    }
}