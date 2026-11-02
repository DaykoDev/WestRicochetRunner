using System.Collections.Generic;
using UnityEngine;

public sealed class BallCollisionResolver
{
    private const int MaxPenetrationPasses = 3;

    private readonly Transform _transform;
    private readonly SphereCollider _sphereCollider;
    private readonly BallMovementSettings _settings;

    private readonly RaycastHit[] _hits;
    private readonly Collider[] _overlapHits;

    private readonly HashSet<Collider> _contactColliders = new();
    private readonly List<Collider> _finishedContacts = new();

    public BallCollisionResolver(Transform ballTransform, SphereCollider sphereCollider, BallMovementSettings settings)
    {
        _transform = ballTransform;
        _sphereCollider = sphereCollider;
        _settings = settings;

        _sphereCollider.radius = _settings.Radius;
        _sphereCollider.isTrigger = true;

        _hits = new RaycastHit[_settings.CastBufferSize];
        _overlapHits = new Collider[_settings.CastBufferSize];
    }

    public void Reset()
    {
        _contactColliders.Clear();
        _finishedContacts.Clear();
    }

    public void PrepareStep()
    {
        UpdateContactColliders();
        ResolvePenetration();
    }

    public bool TryGetCollision(Vector3 direction, float distance, out RaycastHit hit, out Vector3 normal)
    {
        return BallCollisionQuery.SphereCast(
            _transform.position,
            _settings.Radius,
            direction,
            distance,
            _hits,
            _settings.CollisionMask,
            out hit,
            out normal,
            _contactColliders);
    }

    public void RegisterContact(Collider collider)
    {
        if (collider != null)
        {
            _contactColliders.Add(collider);
        }
    }

    public float Separate(RaycastHit hit, Vector3 normal)
    {
        float separation = BallCollisionQuery.GetSeparationDistance(hit, _settings);

        _transform.position += normal * separation;

        return separation;
    }

    private void ResolvePenetration()
    {
        for (int pass = 0; pass < MaxPenetrationPasses; pass++)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                _transform.position,
                _settings.Radius,
                _overlapHits,
                _settings.CollisionMask,
                QueryTriggerInteraction.Ignore);

            bool hasPenetration = false;

            for (int i = 0; i < hitCount; i++)
            {
                Collider other = _overlapHits[i];

                if (other == null || other == _sphereCollider)
                {
                    continue;
                }

                bool hasOverlap = Physics.ComputePenetration(
                    _sphereCollider,
                    _transform.position,
                    _transform.rotation,
                    other,
                    other.transform.position,
                    other.transform.rotation,
                    out Vector3 separationDirection,
                    out float separationDistance);

                if (hasOverlap == false || separationDistance <= 0f)
                {
                    continue;
                }

                _transform.position += separationDirection * (separationDistance + _settings.CollisionOffset);

                hasPenetration = true;
            }

            if (hasPenetration == false)
            {
                return;
            }
        }
    }

    private void UpdateContactColliders()
    {
        foreach (Collider contact in _contactColliders)
        {
            if (CanReleaseContact(contact))
            {
                _finishedContacts.Add(contact);
            }
        }

        foreach (Collider contact in _finishedContacts)
        {
            _contactColliders.Remove(contact);
        }

        _finishedContacts.Clear();
    }

    private bool CanReleaseContact(Collider contact)
    {
        if (contact == null ||
            contact.enabled == false ||
            contact.gameObject.activeInHierarchy == false)
        {
            return true;
        }

        float releaseDistance = _settings.Radius + _settings.CollisionOffset * 3f;

        Vector3 closestPoint = contact.ClosestPoint(_transform.position);

        float sqrDistance = (_transform.position - closestPoint).sqrMagnitude;

        return sqrDistance > releaseDistance * releaseDistance;
    }
}