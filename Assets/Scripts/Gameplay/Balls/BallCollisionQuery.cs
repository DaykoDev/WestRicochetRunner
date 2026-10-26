using UnityEngine;

public static class BallCollisionQuery
{
    public const float ContactEpsilon = 0.0001f;

    private const float NormalEpsilon = 0.000001f;

    public static bool SphereCast(
        Vector3 origin,
        float radius,
        Vector3 direction,
        float distance,
        RaycastHit[] results,
        LayerMask collisionMask,
        out RaycastHit closestHit,
        out Vector3 collisionNormal)
    {
        closestHit = default;
        collisionNormal = Vector3.zero;

        if (direction.sqrMagnitude < Mathf.Epsilon)
        {
            return false;
        }

        direction.Normalize();

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            radius,
            direction,
            results,
            distance,
            collisionMask,
            QueryTriggerInteraction.Ignore);

        if (hitCount <= 0)
        {
            return false;
        }

        int closestIndex = -1;
        float closestDistance = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = results[i];

            if (hit.collider == null)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestIndex = i;
            }
        }

        if (closestIndex < 0)
        {
            return false;
        }

        closestHit = results[closestIndex];

        Vector3 normal = closestHit.normal;

        if (normal.sqrMagnitude >= NormalEpsilon)
        {
            normal.Normalize();
        }
        else
        {
            normal = -direction;
        }

        if (Vector3.Dot(direction, normal) > 0f)
        {
            normal = -normal;
        }

        collisionNormal = normal;
        return true;
    }

    public static float GetSeparationDistance(RaycastHit hit, BallMovementSettings settings)
    {
        if (hit.distance <= ContactEpsilon)
        {
            return settings.Radius + settings.CollisionOffset;
        }

        return settings.CollisionOffset;
    }
}