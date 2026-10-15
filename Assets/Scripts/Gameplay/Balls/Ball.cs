using System;
using UnityEngine;

public class Ball : MonoBehaviour, IPoolable<Ball>
{
    [Header("Settings")]
    [SerializeField] private BallMovementSettings _movementSettings;

    [Header("Lifetime")]
    [SerializeField, Min(0.1f)] private float _maxLifetime = 5f;

    private RaycastHit[] _hits;

    private BallStats _stats;
    private Vector3 _direction;

    private float _lifetimeTimer;
    private float _pendingDistance;

    private int _bounceCount;
    private bool _isActive;

    private Collider[] _gateHits;

    public int LaunchId { get; private set; }

    public event Action<Ball> Died;
    public event Action<Ball> ReturnedToPool;

    public bool IsActive => _isActive;

    private void Awake()
    {
        _hits = new RaycastHit[_movementSettings.CastBufferSize];
        _gateHits = new Collider[_movementSettings.CastBufferSize];
    }

    private void FixedUpdate()
    {
        if (_isActive == false)
        {
            return;
        }

        _lifetimeTimer -= Time.fixedDeltaTime;

        if (_lifetimeTimer <= 0f)
        {
            Die();
            return;
        }

        float distance = _stats.Speed * Time.fixedDeltaTime + _pendingDistance;

        _pendingDistance = 0f;

        Move(distance);
    }

    public bool Launch(Vector3 position, Vector3 direction, BallStats stats)
    {
        if (enabled == false || direction.sqrMagnitude < Mathf.Epsilon || stats.Speed <= 0f)
        {
            return false;
        }

        _stats = stats;

        _stats.MaxBounces = Mathf.Clamp(_stats.MaxBounces, 0, _movementSettings.MaxSupportedBounces);

        _direction = direction.normalized;
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
        _isActive = false;
        _lifetimeTimer = 0f;
        _pendingDistance = 0f;
        _bounceCount = 0;
        _direction = Vector3.zero;
        _stats = default(BallStats);

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
                Vector3 endPosition = transform.position + _direction * remainingDistance;
                ChargeGatesAlongSegment(transform.position, endPosition);
                transform.position = endPosition;
                return;
            }

            float traveledDistance = Mathf.Max(0f, hit.distance);

            Vector3 hitCenter = transform.position + _direction * traveledDistance;
            ChargeGatesAlongSegment(transform.position, hitCenter);
            transform.position = hitCenter;

            remainingDistance -= traveledDistance;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(_stats.Damage);
            }

            if (_bounceCount >= _stats.MaxBounces)
            {
                Die();
                return;
            }

            _direction = BallRicochet.CalculateDirection(_direction, normal, _movementSettings.MinBounceAngle);

            float offset = _movementSettings.CollisionOffset;

            transform.position += normal * offset;

            remainingDistance = Mathf.Max(0f, remainingDistance - offset);

            _bounceCount++;
            collisionCount++;
        }

        float maxPendingDistance = _stats.Speed * Time.fixedDeltaTime;

        _pendingDistance = Mathf.Min(remainingDistance, maxPendingDistance);
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
                continue;

            UpgradeGateCharger charger = _gateHits[i].GetComponentInParent<UpgradeGateCharger>();
            charger?.TryCharge(this);
        }
    }

    private void Die()
    {
        if (_isActive == false)
        {
            return;
        }

        _isActive = false;
        _pendingDistance = 0f;

        Died?.Invoke(this);
    }
}