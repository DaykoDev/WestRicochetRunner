using System;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    private const int PreviewBounceCount = 1;

    [Header("Settings")]
    [SerializeField] private BallMovementSettings _movementSettings;

    [Header("Trajectory")]
    [SerializeField, Min(0.1f)] private float _maxDistance = 30f;

    private RaycastHit[] _raycastHits;
    private Vector3[] _trajectoryPoints;

    public int PointCount { get; private set; }
    public int MaxPointCount => PreviewBounceCount + 2;

    private void Awake()
    {
        _raycastHits = new RaycastHit[_movementSettings.CastBufferSize];
        _trajectoryPoints = new Vector3[MaxPointCount];
    }

    public int CalculateTrajectory(Vector3 origin, Vector3 velocity, BallStats stats)
    {
        PointCount = 0;
        float speed = velocity.magnitude;

        if (enabled == false || speed < Mathf.Epsilon)
        {
            return PointCount;
        }

        int maxBounces = Mathf.Min(stats.MaxBounces, PreviewBounceCount);
        Vector3 direction = velocity / speed;
        Vector3 simulationOrigin = origin;
        float remainingDistance = _maxDistance;
        int bounceCount = 0;

        _trajectoryPoints[PointCount++] = origin;

        while (remainingDistance > 0f && PointCount < _trajectoryPoints.Length)
        {
            bool hasHit = BallCollisionQuery.SphereCast(
                simulationOrigin,
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
                _trajectoryPoints[PointCount++] = simulationOrigin + direction * remainingDistance;
                break;
            }

            float traveledDistance = Mathf.Max(0f, hit.distance);
            Vector3 hitCenter = simulationOrigin + direction * traveledDistance;

            _trajectoryPoints[PointCount++] = hitCenter;
            remainingDistance = Mathf.Max(0f, remainingDistance - traveledDistance);

            if (bounceCount >= maxBounces)
            {
                break;
            }

            direction = BallRicochet.CalculateDirection(direction, normal, _movementSettings.MinBounceAngle);

            float separation = BallCollisionQuery.GetSeparationDistance(hit, _movementSettings);

            simulationOrigin = hitCenter + normal * separation;

            remainingDistance = Mathf.Max(0f, remainingDistance - separation);

            bounceCount++;

            if (hit.distance <= BallCollisionQuery.ContactEpsilon)
            {
                break;
            }
        }

        return PointCount;
    }

    public void GetTrajectoryPoints(Vector3[] destinationArray)
    {
        int count = Mathf.Min(PointCount, destinationArray.Length);

        Array.Copy(_trajectoryPoints, destinationArray, count);
    }
}