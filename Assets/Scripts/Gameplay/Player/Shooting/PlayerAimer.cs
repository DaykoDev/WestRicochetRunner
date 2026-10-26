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

    public Vector3 FirePointPosition => _firePoint.position;

    private Vector3 AimOriginPosition =>
        _aimOrigin != null
            ? _aimOrigin.position
            : transform.position;

    public Vector3 AimPointPosition
    {
        get
        {
            Vector3 origin = AimOriginPosition;

            return new Vector3(
                _trackCenterX,
                origin.y + _aimHeight,
                origin.z + _aimDistance);
        }
    }

    public Vector3 Direction
    {
        get
        {
            Vector3 direction =
                AimPointPosition - AimOriginPosition;

            return direction.normalized;
        }
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