using System;
using UnityEngine;

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

    [Header("Ricochet")]
    private BallRicochetVisual _ricochetVisual;
    private BallReturnFeedback _returnFeedback;

    private BallGateInteraction _gateInteraction;

    private RaycastHit[] _hits;

    private float _returnTimer;
    private bool _isReturning;

    private BallStats _stats;
    private Vector3 _direction;
    private float _effectiveSpeed;
    private float _pendingDistance;

    private Camera _camera;
    private float _lifetimeTimer;
    private int _bounceCount;
    private bool _isActive;
    private IDamageable _lastDamaged;

    private float _launchSpeed;

    public int LaunchId { get; private set; }

    public event Action<Ball> Died;
    public event Action<Ball> ReturnedToPool;
    public event Action<Vector3> Ricocheted;

    public bool IsActive => _isActive;

    private void Awake()
    {
        _hits = new RaycastHit[_movementSettings.CastBufferSize];
        _gateInteraction = new BallGateInteraction(_movementSettings);
        _returnFeedback = GetComponent<BallReturnFeedback>();
        _ricochetVisual = GetComponent<BallRicochetVisual>();
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

        _lifetimeTimer -= Time.fixedDeltaTime;

        if (_lifetimeTimer <= 0f)
        {
            BeginReturn();
            return;
        }

        float distance = _effectiveSpeed * Time.fixedDeltaTime + _pendingDistance;
        _pendingDistance = 0f;

        Move(distance);
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

        _isReturning = false;
        _returnTimer = 0f;

        _returnFeedback.Restore();

        _stats = stats;

        _stats.MaxBounces = Mathf.Clamp(_stats.MaxBounces, 0, _movementSettings.MaxSupportedBounces);

        _launchSpeed = velocity.magnitude;
        _effectiveSpeed = _launchSpeed;

        _ricochetVisual.ResetVisual();

        _direction = velocity.normalized;
        _bounceCount = 0;
        _lastDamaged = null;
        _lifetimeTimer = _maxLifetime;
        _pendingDistance = 0f;
        _isActive = true;

        FaceDirection();

        transform.position = position;

        unchecked { LaunchId++; }

        return true;
    }

    public void OnReturnedToPool()
    {

        _isActive = false;
        _isReturning = false;
        _lifetimeTimer = 0f;
        _pendingDistance = 0f;
        _effectiveSpeed = 0f;
        _launchSpeed = 0f;
        _bounceCount = 0;
        _direction = Vector3.zero;
        _stats = default;

        ReturnedToPool?.Invoke(this);
    }

    private void Move(float distance)
    {
        float remainingDistance = distance;
        int collisionCount = 0;

        while (remainingDistance > 0f && collisionCount < _movementSettings.MaxCollisionsPerStep)
        {
            bool hasHit = BallCollisionQuery.SphereCast(
                transform.position,
                _movementSettings.Radius,
                _direction,
                remainingDistance,
                _hits,
                _movementSettings.CollisionMask,
                out RaycastHit hit,
                out Vector3 normal
            );

            if (hasHit == false)
            {
                Vector3 startPosition = transform.position;
                Vector3 endPosition = startPosition + _direction * remainingDistance;

                _gateInteraction.ChargeAlongSegment(this, startPosition, endPosition);

                transform.position = endPosition;
                return;
            }

            bool hasNoMovementProgress = hit.distance <= BallCollisionQuery.ContactEpsilon;

            if (hasNoMovementProgress && collisionCount > 0)
            {
                _pendingDistance = 0f;
                return;
            }

            float traveledDistance = Mathf.Max(0f, hit.distance);
            Vector3 hitCenter = transform.position + _direction * traveledDistance;

            _gateInteraction.ChargeAlongSegment(this, transform.position, hitCenter);

            transform.position = hitCenter;

            remainingDistance -= traveledDistance;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null && ReferenceEquals(damageable, _lastDamaged) == false)
            {
                damageable.TakeDamage(_stats.Damage);
            }

            _lastDamaged = damageable;

            _direction = BallRicochet.CalculateDirection(_direction, normal, _movementSettings.MinBounceAngle);
            FaceDirection();

            _bounceCount++;
            collisionCount++;

            float maxSpeed = _launchSpeed * _movementSettings.MaxSpeedMultiplierAfterRicochet;

            _effectiveSpeed = Mathf.Min(_effectiveSpeed * _movementSettings.SpeedMultiplierAfterRicochet, maxSpeed);

            _ricochetVisual.OnRicochet();

            Ricocheted?.Invoke(transform.position);

            float separation = BallCollisionQuery.GetSeparationDistance(hit, _movementSettings);

            transform.position += normal * separation;

            remainingDistance = Mathf.Max(0f, remainingDistance - separation);

            if (_bounceCount > _stats.MaxBounces)
            {
                BeginReturn();
                return;
            }

            if (hasNoMovementProgress)
            {
                _pendingDistance = 0f;
                return;
            }
        }

        float maxPendingDistance = _effectiveSpeed * Time.fixedDeltaTime;
        _pendingDistance = Mathf.Min(remainingDistance, maxPendingDistance);
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

    private void BeginReturn(bool allowEffect = true)
    {
        if (_isActive == false || _isReturning)
        {
            return;
        }

        _isActive = false;
        _isReturning = true;

        bool shouldPlayEffect = allowEffect && CanPlayReturnEffect();

        _returnFeedback.Play(transform.position, shouldPlayEffect);

        _pendingDistance = 0f;
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

    private void FaceDirection()
    {
        if (_direction.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
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