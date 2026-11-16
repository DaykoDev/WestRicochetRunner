using System.Collections.Generic;
using UnityEngine;
using WestRicochet.Gameplay.Balls.Movement;

namespace WestRicochet.Gameplay.Balls.Collision
{
    public static class BallCollisionQuery
    {
        public const float ContactEpsilon = 0.0001f;
        private const float NormalEpsilon = 0.000001f;
        private const int MaxOverflowAttempts = 4;
        private const int BufferGrowthFactor = 2;

        private static RaycastHit[] OverflowHits;

        public static bool TrySphereCast(
            Vector3 origin,
            float radius,
            Vector3 direction,
            float distance,
            RaycastHit[] results,
            LayerMask collisionMask,
            out RaycastHit closestHit,
            out Vector3 collisionNormal,
            HashSet<Collider> ignoredColliders = null)
        {
            closestHit = default;
            collisionNormal = Vector3.zero;

            if (direction.sqrMagnitude < Mathf.Epsilon)
            {
                return false;
            }

            direction.Normalize();

            RaycastHit[] hits = results;

            int hitCount = Physics.SphereCastNonAlloc(
                origin,
                radius,
                direction,
                hits,
                distance,
                collisionMask,
                QueryTriggerInteraction.Ignore);

            int attempt = 0;

            while (hitCount > 0 &&
                   hitCount == hits.Length &&
                   attempt < MaxOverflowAttempts)
            {
                int requiredSize = hits.Length * BufferGrowthFactor;

                if (OverflowHits == null || OverflowHits.Length < requiredSize)
                {
                    OverflowHits = new RaycastHit[requiredSize];
                }

                hits = OverflowHits;

                hitCount = Physics.SphereCastNonAlloc(
                    origin,
                    radius,
                    direction,
                    hits,
                    distance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore);

                attempt++;
            }

            if (hitCount <= 0)
            {
                return false;
            }

            int closestIndex = -1;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.collider == null)
                {
                    continue;
                }

                if (ignoredColliders != null && ignoredColliders.Contains(hit.collider))
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

            closestHit = hits[closestIndex];

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

        public static float GetSeparationDistance(BallMovementSettings settings)
        {
            return settings.CollisionOffset;
        }
    }
}