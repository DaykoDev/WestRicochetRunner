using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(BallReturnFeedback))]
[RequireComponent(typeof(BallRicochetVisual))]
public class Ball : MonoBehaviour, IPoolable<Ball>
{
    [Header("Settings")]
    [SerializeField] private BallMovementSettings _movementSettings;

    [Header("Lifetime")]
    [SerializeField, Min(0.1f)] private float _maxLifetime = 5f;

    [Header("Return")]
    [SerializeField, Min(0.1f)] private float _returnDelay = 1f;

    [SerializeField, Range(0f, 0.5f)] private float _returnEffectMinViewportY = 0.15f;

    private BallMovement _movement;
    private BallCollisionResolver _collisionResolver;

    private BallRicochetVisual _ricochetVisual;
    private BallReturnFeedback _returnFeedback;

    private Camera _camera;

    private float _lifetimeTimer;
    private float _returnTimer;

    private bool _isActive;
    private bool _isReturning;

    public int LaunchId { get; private set; }

    public bool IsActive => _isActive;

    public event Action<Ball> Died;
    public event Action<Ball> ReturnedToPool;
    public event Action<Vector3> Ricocheted;

    private void Awake()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        _collisionResolver = new BallCollisionResolver(transform, sphereCollider, _movementSettings);

        _movement = new BallMovement(this, transform, _movementSettings, _collisionResolver);

        _movement.Ricocheted += OnMovementRicocheted;
        _movement.BouncesExhausted += OnBouncesExhausted;

        _returnFeedback = GetComponent<BallReturnFeedback>();
        _ricochetVisual = GetComponent<BallRicochetVisual>();
    }

    private void OnDestroy()
    {
        if (_movement == null)
        {
            return;
        }

        _movement.Ricocheted -= OnMovementRicocheted;
        _movement.BouncesExhausted -= OnBouncesExhausted;
    }

    private void FixedUpdate()
    {
        if (_isReturning)
        {
            UpdateReturnDelay();
            return;
        }

        if (_isActive == false)
        {
            return;
        }

        if (IsOutsideScreen())
        {
            BeginReturn(false);
            return;
        }

        UpdateLifetime();
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void SetEffectPool(EffectPool effectPool)
    {
        _returnFeedback.SetEffectPool(effectPool);
    }

    public bool Launch(Vector3 position, Vector3 velocity, BallStats stats)
    {
        if (enabled == false || velocity.sqrMagnitude < Mathf.Epsilon || stats.Speed <= 0f)
        {
            return false;
        }

        transform.position = position;

        _isReturning = false;
        _returnTimer = 0f;

        _returnFeedback.Restore();
        _ricochetVisual.ResetVisual();

        _movement.Launch(velocity, stats);

        _lifetimeTimer = _maxLifetime;
        _isActive = true;

        FaceDirection();

        unchecked
        {
            LaunchId++;
        }

        return true;
    }

    public void OnReturnedToPool()
    {
        _movement.Reset();

        _isActive = false;
        _isReturning = false;

        _lifetimeTimer = 0f;
        _returnTimer = 0f;

        ReturnedToPool?.Invoke(this);
    }

    private void UpdateLifetime()
    {
        _lifetimeTimer -= Time.fixedDeltaTime;

        if (_lifetimeTimer <= 0f)
        {
            BeginReturn();
            return;
        }

        _movement.Tick(Time.fixedDeltaTime);
    }

    private void OnMovementRicocheted(Vector3 position)
    {
        FaceDirection();
        _ricochetVisual.OnRicochet();

        Ricocheted?.Invoke(position);
    }

    private void OnBouncesExhausted()
    {
        BeginReturn();
    }

    private void FaceDirection()
    {
        Vector3 direction = _movement.Direction;

        if (direction.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void BeginReturn(bool allowEffect = true)
    {
        if (_isActive == false || _isReturning)
        {
            return;
        }

        _isActive = false;
        _isReturning = true;

        _movement.Stop();

        bool shouldPlayEffect = allowEffect && CanPlayReturnEffect();

        _returnFeedback.Play(transform.position, shouldPlayEffect);

        _returnTimer = 0f;
    }

    private void UpdateReturnDelay()
    {
        _returnTimer += Time.fixedDeltaTime;

        if (_returnTimer >= _returnDelay)
        {
            CompleteReturn();
        }
    }

    private void CompleteReturn()
    {
        _isReturning = false;
        Died?.Invoke(this);
    }

    private bool IsOutsideScreen()
    {
        if (_camera == null)
        {
            return false;
        }

        Vector3 viewportPosition = _camera.WorldToViewportPoint(transform.position);

        return viewportPosition.z < 0f || viewportPosition.y < 0f;
    }

    private bool CanPlayReturnEffect()
    {
        if (_camera == null)
        {
            return true;
        }

        Vector3 viewportPosition = _camera.WorldToViewportPoint(transform.position);

        return viewportPosition.z > 0f &&
               viewportPosition.x >= 0f &&
               viewportPosition.x <= 1f &&
               viewportPosition.y >= _returnEffectMinViewportY &&
               viewportPosition.y <= 1f;
    }
}