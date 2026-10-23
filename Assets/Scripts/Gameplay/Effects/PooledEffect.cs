using System.Collections;
using UnityEngine;

public class PooledEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticle;
    [SerializeField, Min(0.1f)] private float _lifetime = 1f;

    private EffectPool _pool;

    public void SetPool(EffectPool pool)
    {
        _pool = pool;
    }

    public void PlayAt(Vector3 position)
    {
        transform.position = position;
        _explosionParticle.Play();
        StartCoroutine(ReleaseAfterLifetime());
    }

    private IEnumerator ReleaseAfterLifetime()
    {
        yield return new WaitForSeconds(_lifetime);
        _pool?.Release(this);
    }
}