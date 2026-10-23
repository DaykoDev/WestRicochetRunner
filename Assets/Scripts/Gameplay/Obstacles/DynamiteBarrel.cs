using UnityEngine;

public class DynamiteBarrel : Obstacle
{
    [Header("Explosion")]
    [SerializeField, Min(0f)] private float _explosionRadius = 2.5f;
    [SerializeField, Min(1)] private int _explosionDamage = 6;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField, Min(4)] private int _hitBufferSize = 16;

    private EffectPool _explosionPool;
    private Collider[] _hitBuffer;

    private void Awake()
    {
        _hitBuffer = new Collider[_hitBufferSize];
    }

    public void SetExplosionPool(EffectPool explosionPool)
    {
        _explosionPool = explosionPool;
    }

    protected override void OnDied()
    {
        if (_explosionPool != null)
        {
            _explosionPool.Get().PlayAt(transform.position);
        }

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            _explosionRadius,
            _hitBuffer,
            _obstacleMask,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            Obstacle other = _hitBuffer[i].GetComponentInParent<Obstacle>();

            if (other == null || other == this || other.IsDestroyed)
            {
                continue;
            }

            other.TakeDamage(_explosionDamage);
        }
    }
}