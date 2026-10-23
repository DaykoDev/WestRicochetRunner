using UnityEngine;

public class EffectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private PooledEffect _effectPrefab;
    [SerializeField, Min(1)] private int _initialSize = 8;
    [SerializeField, Min(1)] private int _maxSize = 30;

    private ComponentPool<PooledEffect> _pool;

    private void Awake()
    {
        _pool = new ComponentPool<PooledEffect>(_effectPrefab, transform, _initialSize, _maxSize, OnEffectCreated);
        _pool.Prewarm(_initialSize);
    }

    public PooledEffect Get()
    {
        return _pool.Get();
    }

    public void Release(PooledEffect effect)
    {
        _pool?.Release(effect);
    }

    private void OnEffectCreated(PooledEffect effect)
    {
        effect.SetPool(this);
    }
}