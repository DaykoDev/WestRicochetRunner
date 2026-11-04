using UnityEngine;

public class PlayerAimer : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField, Min(0.1f)] private float _aimDistance = 10f;
    [SerializeField] private float _aimHeight;
    [SerializeField] private float _trackCenterX;

    [Header("References")]
    [SerializeField] private Transform _aimOrigin;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _secondaryFirePoint;

    public Vector3 FirePointPosition => _firePoint.position;

    public bool HasSecondaryFirePoint => _secondaryFirePoint != null;

    public Vector3 SecondaryFirePointPosition => _secondaryFirePoint.position;

    public Vector3 AimPointPosition
    {
        get
        {
            Vector3 origin = AimOriginPosition;

            return new Vector3(_trackCenterX, origin.y + _aimHeight, origin.z + _aimDistance);
        }
    }

    public Vector3 Direction
    {
        get
        {
            Vector3 direction = AimPointPosition - AimOriginPosition;

            return direction.normalized;
        }
    }

    public Vector3 TrajectoryOriginPosition
    {
        get
        {
            if (_isTrajectoryCentered && HasSecondaryFirePoint)
            {
                return (FirePointPosition + SecondaryFirePointPosition) * 0.5f;
            }

            return FirePointPosition;
        }
    }

    private Vector3 AimOriginPosition => _aimOrigin != null ? _aimOrigin.position : transform.position;
    private bool _isTrajectoryCentered;

    public void SetTrajectoryCentered(bool isCentered)
    {
        _isTrajectoryCentered = isCentered;
    }

    private void OnDrawGizmosSelected()
    {
        if (_firePoint == null)
        {
            return;
        }

        Vector3 aimPoint = AimPointPosition;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_firePoint.position, aimPoint);
        Gizmos.DrawWireSphere(aimPoint, 0.2f);
    }
}