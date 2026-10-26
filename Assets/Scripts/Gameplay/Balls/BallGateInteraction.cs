using UnityEngine;

public sealed class BallGateInteraction
{
    private readonly BallMovementSettings _movementSettings;
    private readonly Collider[] _gateHits;

    public BallGateInteraction(BallMovementSettings movementSettings)
    {
        _movementSettings = movementSettings;
        _gateHits = new Collider[movementSettings.CastBufferSize];
    }

    public void ChargeAlongSegment(
        Ball ball,
        Vector3 start,
        Vector3 end)
    {
        int hitCount = Physics.OverlapCapsuleNonAlloc(
            start,
            end,
            _movementSettings.Radius,
            _gateHits,
            _movementSettings.UpgradeGateMask,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < hitCount; i++)
        {
            Collider gateCollider = _gateHits[i];

            if (gateCollider == null)
            {
                continue;
            }

            UpgradeGateCharger charger =
                gateCollider.GetComponentInParent<UpgradeGateCharger>();

            charger?.TryCharge(ball);
        }
    }
}