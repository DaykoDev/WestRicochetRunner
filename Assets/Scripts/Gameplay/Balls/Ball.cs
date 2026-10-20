using System;
using UnityEngine;

public class Ball : MonoBehaviour, IPoolable<Ball>
{
    [Header("Settings")]
    [SerializeField] private BallMovementSettings _movementSettings;

    [Header("Lifetime")]
    [SerializeField, Min(0.1f)] private float _maxLifetime = 5f;

    [Header("Return Animation")]
    [SerializeField] private BallReturnVisual _returnVisual;
    [SerializeField, Min(0.1f)] private float _returnDuration = 1f;

    private Transform _returnTarget;
    private Vector3 _returnStartPosition;
    private float _returnTimer;
    private bool _isReturning;

    private RaycastHit[] _hits;
    private Collider[] _gateHits;

    private BallStats _stats;
    private Vector3 _direction;
    private float _effectiveSpeed;
    private float _pendingDistance;

    private Camera _camera;
    private float _lifetimeTimer;
    private int _bounceCount;
    private bool _isActive;

    public int LaunchId { get; private set; }

    public event Action<Ball> Died;
    public event Action<Ball> ReturnedToPool;

    public bool IsActive => _isActive;

    private void Awake()
    {
        _camera = Camera.main;
        _hits = new RaycastHit[_movementSettings.CastBufferSize];
        _gateHits = new Collider[_movementSettings.CastBufferSize];
    }

    private void FixedUpdate()
    {
        if (_isReturning)
        {
            UpdateReturnAnimation();
            return;
        }

        if (_isActive == false)
        {
            return;
        }

        if (IsOutsideScreen())
        {
            BeginReturn();
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

    public bool Launch(Vector3 position, Vector3 velocity, BallStats stats, Transform returnTarget)
    {
        if (enabled == false || velocity.sqrMagnitude < Mathf.Epsilon || stats.Speed <= 0f)
        {
            return false;
        }

        _returnVisual?.ResetVisual();

        _returnTarget = returnTarget;
        _isReturning = false;
        _returnTimer = 0f;
        _stats = stats;

        _stats.MaxBounces = Mathf.Clamp(_stats.MaxBounces, 0, _movementSettings.MaxSupportedBounces);

        _effectiveSpeed = velocity.magnitude;
        _direction = velocity.normalized;
        _bounceCount = 0;
        _lifetimeTimer = _maxLifetime;
        _pendingDistance = 0f;
        _isActive = true;

        transform.position = position;

        unchecked { LaunchId++; }

        return true;
    }

    public void OnReturnedToPool()
    {
        _returnVisual?.ResetVisual();

        _isActive = false;
        _isReturning = false;
        _returnTarget = null;
        _returnTimer = 0f;
        _lifetimeTimer = 0f;
        _pendingDistance = 0f;
        _effectiveSpeed = 0f;
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

                ChargeGatesAlongSegment(startPosition, endPosition);
                transform.position = endPosition;
                return;
            }

            float traveledDistance = Mathf.Max(0f, hit.distance);
            Vector3 hitCenter = transform.position + _direction * traveledDistance;

            ChargeGatesAlongSegment(transform.position, hitCenter);
            transform.position = hitCenter;

            remainingDistance -= traveledDistance;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            damageable?.TakeDamage(_stats.Damage);

            _direction = BallRicochet.CalculateDirection(_direction, normal, _movementSettings.MinBounceAngle);

            float offset = _movementSettings.CollisionOffset;
            transform.position += normal * offset;
            remainingDistance = Mathf.Max(0f, remainingDistance - offset);

            _bounceCount++;
            collisionCount++;

            if (_bounceCount >= _stats.MaxBounces)
            {
                BeginReturn();
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

    private void BeginReturn()
    {
        if (_isActive == false || _isReturning)
        {
            return;
        }

        _isActive = false;
        _isReturning = true;
        _returnVisual?.SetReturning();

        _pendingDistance = 0f;
        _returnTimer = 0f;
        _returnStartPosition = transform.position;
    }

    private void UpdateReturnAnimation()
    {
        if (_returnTarget == null)
        {
            CompleteReturn();
            return;
        }

        _returnTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(_returnTimer / _returnDuration);
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

        transform.position = Vector3.Lerp(_returnStartPosition, _returnTarget.position, easedProgress);

        if (progress >= 1f)
        {
            CompleteReturn();
        }
    }

    private void CompleteReturn()
    {
        _isReturning = false;
        Died?.Invoke(this);
    }

    private void ChargeGatesAlongSegment(Vector3 start, Vector3 end)
    {
        int hitCount = Physics.OverlapCapsuleNonAlloc(
            start,
            end,
            _movementSettings.Radius,
            _gateHits,
            _movementSettings.UpgradeGateMask,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (_gateHits[i] == null)
            {
                continue;
            }

            UpgradeGateCharger charger = _gateHits[i].GetComponentInParent<UpgradeGateCharger>();
            charger?.TryCharge(this);
        }
    }
}