using UnityEngine;

public static class BallRicochet
{
    public static Vector3 GetFacingNormal(Vector3 incomingDirection, Vector3 normal)
    {
        normal.Normalize();

        if (Vector3.Dot(incomingDirection, normal) > 0f)
        {
            normal = -normal;
        }

        return normal;
    }

    public static Vector3 CalculateDirection(Vector3 incomingDirection, Vector3 normal, float minBounceAngle)
    {
        if (incomingDirection.sqrMagnitude < Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        incomingDirection.Normalize();

        normal = GetFacingNormal(incomingDirection, normal);

        Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, normal).normalized;

        minBounceAngle = Mathf.Clamp(minBounceAngle, 0f, 89f);

        float angleToNormal = Vector3.Angle(reflectedDirection, normal);

        float maxAngleToNormal = 90f - minBounceAngle;

        if (angleToNormal > maxAngleToNormal)
        {
            Vector3 tangent = Vector3.ProjectOnPlane(reflectedDirection, normal);

            if (tangent.sqrMagnitude > Mathf.Epsilon)
            {
                tangent.Normalize();

                float angleRadians = minBounceAngle * Mathf.Deg2Rad;

                reflectedDirection = tangent * Mathf.Cos(angleRadians) + normal * Mathf.Sin(angleRadians);
            }
        }

        return reflectedDirection.normalized;
    }
}