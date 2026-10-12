using System;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    private const int PreviewBounceCount = 1;

    [Header("Settings")]
    [SerializeField] private BallMovementSettings _movementSettings;

    [Header("Trajectory")]
    [SerializeField, Min(0.1f)] private float _maxDistance = 50f;

    private RaycastHit[] _raycastHits;
    private Vector3[] _trajectoryPoints;

    public int PointCount { get; private set; }
    public int MaxPointCount => PreviewBounceCount + 2;

    private void Awake()
    {
        _raycastHits = new RaycastHit[_movementSettings.CastBufferSize];
        _trajectoryPoints = new Vector3[MaxPointCount];
    }

    public int CalculateTrajectory(Vector3 origin, Vector3 direction, BallStats stats)
    {
        PointCount = 0;

        if (enabled == false || direction.sqrMagnitude < Mathf.Epsilon)
        {
            return PointCount;
        }

        int maxBounces = Mathf.Min(stats.MaxBounces, PreviewBounceCount);

        direction.Normalize();

        float remainingDistance = _maxDistance;
        int bounceCount = 0;

        _trajectoryPoints[PointCount] = origin;
        PointCount++;

        while (remainingDistance > 0f && PointCount < _trajectoryPoints.Length)
        {
            bool hasHit = BallCollisionQuery.SphereCast(
                origin,
                _movementSettings.Radius,
                direction,
                remainingDistance,
                _raycastHits,
                _movementSettings.CollisionMask,
                out RaycastHit hit,
                out Vector3 normal
             );

            if (hasHit == false)
            {
                _trajectoryPoints[PointCount++] = origin + direction * remainingDistance;

                break;
            }

            Vector3 hitCenter = origin + direction * hit.distance;

            _trajectoryPoints[PointCount++] = hitCenter;

            remainingDistance = Mathf.Max(0f, remainingDistance - hit.distance);

            if (bounceCount >= maxBounces)
            {
                break;
            }

            direction = BallRicochet.CalculateDirection(direction, normal, _movementSettings.MinBounceAngle);

            float offset = _movementSettings.CollisionOffset;

            origin = hitCenter + normal * offset;

            remainingDistance = Mathf.Max(0f, remainingDistance - offset);

            bounceCount++;
        }

        return PointCount;
    }

    public void GetTrajectoryPoints(Vector3[] destinationArray)
    {
        int count = Mathf.Min(PointCount, destinationArray.Length);

        Array.Copy(_trajectoryPoints, destinationArray, count);
    }
}