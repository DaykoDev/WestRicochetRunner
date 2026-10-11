using UnityEngine;

public static class BallCollisionQuery
{
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
        closestHit = default(RaycastHit);
        collisionNormal = Vector3.zero;

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            radius,
            direction,
            results,
            distance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

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

        Vector3 hitCenter = origin + direction * closestHit.distance;

        Vector3 normal = hitCenter - closestHit.point;

        if (normal.sqrMagnitude > 0.000001f)
        {
            normal.Normalize();
        }
        else
        {
            normal = closestHit.normal.normalized;
        }

        if (Vector3.Dot(direction, normal) > 0f)
        {
            normal = -normal;
        }

        collisionNormal = normal;

        return true;
    }
}