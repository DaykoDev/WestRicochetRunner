using System;
using UnityEngine;

public sealed class BallMovement
{
    private readonly Ball _ball;
    private readonly Transform _transform;
    private readonly BallMovementSettings _settings;
    private readonly BallCollisionResolver _collisionResolver;
    private readonly BallGateInteraction _gateInteraction;

    private BallStats _stats;
    private Vector3 _direction;

    private float _launchSpeed;
    private float _effectiveSpeed;
    private float _pendingDistance;
    private float _planeY;

    private int _bounceCount;
    private IDamageable _lastDamaged;

    public BallMovement(Ball ball, Transform ballTransform, BallMovementSettings settings, BallCollisionResolver collisionResolver)
    {
        _ball = ball;
        _transform = ballTransform;
        _settings = settings;
        _collisionResolver = collisionResolver;

        _gateInteraction = new BallGateInteraction(settings);
    }

    public Vector3 Direction => _direction;

    public event Action<Vector3> Ricocheted;
    public event Action BouncesExhausted;

    public void Launch(Vector3 velocity, BallStats stats)
    {
        _stats = stats;
        _planeY = _transform.position.y;

        _stats.MaxBounces = Mathf.Clamp(_stats.MaxBounces, 0, _settings.MaxSupportedBounces);

        _launchSpeed = velocity.magnitude;
        _effectiveSpeed = _launchSpeed;
        _direction = Vector3.ProjectOnPlane(velocity, Vector3.up).normalized;

        _bounceCount = 0;
        _lastDamaged = null;
        _pendingDistance = 0f;

        _collisionResolver.Reset();
    }

    public void Tick(float deltaTime)
    {
        float distance = _effectiveSpeed * deltaTime + _pendingDistance;

        _pendingDistance = 0f;

        Move(distance, deltaTime);

        Vector3 position = _transform.position;
        position.y = _planeY;
        _transform.position = position;
    }

    public void Stop()
    {
        _pendingDistance = 0f;
    }

    public void Reset()
    {
        _stats = default;
        _direction = Vector3.zero;

        _launchSpeed = 0f;
        _effectiveSpeed = 0f;
        _pendingDistance = 0f;

        _bounceCount = 0;
        _lastDamaged = null;

        _collisionResolver.Reset();
    }

    private void Move(float distance, float deltaTime)
    {
        _collisionResolver.PrepareStep();

        float remainingDistance = distance;
        int collisionCount = 0;

        while (remainingDistance > 0f && collisionCount < _settings.MaxCollisionsPerStep)
        {
            bool hasHit = _collisionResolver.TryGetCollision(
                _direction,
                remainingDistance,
                out RaycastHit hit,
                out Vector3 normal);

            if (hasHit == false)
            {
                MoveWithoutCollision(remainingDistance);
                return;
            }

            bool hasNoMovementProgress = hit.distance <= BallCollisionQuery.ContactEpsilon;

            if (hasNoMovementProgress && collisionCount > 0)
            {
                Stop();
                return;
            }

            MoveToCollision(hit);

            remainingDistance -= Mathf.Max(0f, hit.distance);

            _collisionResolver.RegisterContact(hit.collider);

            ApplyDamage(hit.collider);
            ApplyRicochet(normal);

            _bounceCount++;
            collisionCount++;

            Ricocheted?.Invoke(_transform.position);

            float separation = _collisionResolver.Separate(hit, normal);

            remainingDistance = Mathf.Max(0f, remainingDistance - separation);

            if (_bounceCount > _stats.MaxBounces)
            {
                BouncesExhausted?.Invoke();
                return;
            }

            if (hasNoMovementProgress)
            {
                Stop();
                return;
            }
        }

        float maxPendingDistance = _effectiveSpeed * deltaTime;

        _pendingDistance = Mathf.Min(remainingDistance, maxPendingDistance);
    }

    private void MoveWithoutCollision(float distance)
    {
        Vector3 startPosition = _transform.position;
        Vector3 endPosition = startPosition + _direction * distance;

        _gateInteraction.ChargeAlongSegment(_ball, startPosition, endPosition);

        _transform.position = endPosition;
    }

    private void MoveToCollision(RaycastHit hit)
    {
        float traveledDistance = Mathf.Max(0f, hit.distance);

        Vector3 hitCenter = _transform.position + _direction * traveledDistance;

        _gateInteraction.ChargeAlongSegment(_ball, _transform.position, hitCenter);

        _transform.position = hitCenter;
    }

    private void ApplyDamage(Collider collider)
    {
        IDamageable damageable = collider.GetComponentInParent<IDamageable>();

        if (damageable != null && ReferenceEquals(damageable, _lastDamaged) == false)
        {
            damageable.TakeDamage(_stats.Damage);
        }

        _lastDamaged = damageable;
    }

    private void ApplyRicochet(Vector3 normal)
    {
        _direction = BallRicochet.CalculateDirection(_direction, normal, _settings.MinBounceAngle);

        float maxSpeed = _launchSpeed * _settings.MaxSpeedMultiplierAfterRicochet;

        _effectiveSpeed = Mathf.Min( _effectiveSpeed * _settings.SpeedMultiplierAfterRicochet, maxSpeed);
    }
}