using UnityEngine;

[CreateAssetMenu(fileName = "BallMovementSettings", menuName = "Game/Ball Movement Settings")]
public class BallMovementSettings : ScriptableObject
{
    [Header("Collision")]
    [SerializeField, Min(0.001f)] private float _radius = 0.15f;
    [SerializeField, Min(0.001f)] private float _collisionOffset = 0.01f;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField, Min(1)] private int _castBufferSize = 16;

    [Header("Ricochet")]
    [SerializeField, Range(0f, 89f)] private float _minBounceAngle = 25f;
    [SerializeField, Min(1f)] private float _speedMultiplierAfterRicochet = 1.12f;

    [SerializeField, Min(1f)] private float _maxSpeedMultiplierAfterRicochet = 2.5f;

    [Header("Movement")]
    [SerializeField, Min(1)] private int _maxCollisionsPerStep = 5;

    [Header("Trajectory Capacity")]
    [SerializeField, Min(0)] private int _maxSupportedBounces = 16;

    [Header("Upgrade Gates")]
    [SerializeField] private LayerMask _upgradeGateMask;

    public int MaxSupportedBounces => _maxSupportedBounces;
    public float Radius => _radius;
    public float CollisionOffset => _collisionOffset;
    public LayerMask CollisionMask => _collisionMask;
    public int CastBufferSize => _castBufferSize;
    public float MinBounceAngle => _minBounceAngle;
    public float SpeedMultiplierAfterRicochet => _speedMultiplierAfterRicochet;
    public float MaxSpeedMultiplierAfterRicochet => _maxSpeedMultiplierAfterRicochet;
    public int MaxCollisionsPerStep => _maxCollisionsPerStep;
    public LayerMask UpgradeGateMask => _upgradeGateMask;
}