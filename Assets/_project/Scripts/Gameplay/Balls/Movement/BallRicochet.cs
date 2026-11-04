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

        incomingDirection = Vector3.ProjectOnPlane(incomingDirection, Vector3.up);

        normal = Vector3.ProjectOnPlane(normal, Vector3.up);

        if (incomingDirection.sqrMagnitude < Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        incomingDirection.Normalize();

        if (normal.sqrMagnitude < Mathf.Epsilon)
        {
            return -incomingDirection;
        }

        normal = GetFacingNormal(incomingDirection, normal);

        Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, normal).normalized;

        minBounceAngle = Mathf.Clamp(minBounceAngle, 0f, 89f);

        float angleToNormal = Vector3.Angle(reflectedDirection, normal);

        float maxAngleToNormal = 90f - minBounceAngle;

        if (angleToNormal > maxAngleToNormal)
        {
            Vector3 tangent = Vector3.ProjectOnPlane(incomingDirection, normal);

            if (tangent.sqrMagnitude > Mathf.Epsilon)
            {
                tangent.Normalize();

                float angleRadians = minBounceAngle * Mathf.Deg2Rad;

                reflectedDirection = tangent * Mathf.Cos(angleRadians) + normal * Mathf.Sin(angleRadians);
            }
        }

        reflectedDirection.y = 0f;

        return reflectedDirection.normalized;
    }
}