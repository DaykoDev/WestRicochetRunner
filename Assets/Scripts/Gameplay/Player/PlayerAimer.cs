
using UnityEngine;

public class PlayerAimer : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField, Min(0.1f)] private float _aimDistance = 10f;
    [SerializeField] private float _aimHeight;
    [SerializeField] private float _trackCenterX;

    [Header("References")]
    [SerializeField] private Transform _firePoint;

    public Vector3 FirePointPosition => _firePoint.position;
    public Transform FirePointTransform => _firePoint;

    public Vector3 AimPointPosition
    {
        get
        {
            Vector3 firePosition = _firePoint.position;

            return new Vector3(_trackCenterX, firePosition.y + _aimHeight, firePosition.z + _aimDistance);
        }
    }

    public Vector3 Direction =>
        (AimPointPosition - FirePointPosition).normalized;

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
