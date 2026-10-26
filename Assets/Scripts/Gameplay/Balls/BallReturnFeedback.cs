using UnityEngine;

public sealed class BallReturnFeedback : MonoBehaviour
{
    [SerializeField] private GameObject _visualRoot;

    private EffectPool _effectPool;

    public void SetEffectPool(EffectPool effectPool)
    {
        _effectPool = effectPool;
    }

    public void Restore()
    {
        if (_visualRoot != null)
        {
            _visualRoot.SetActive(true);
        }
    }

    public void Play(Vector3 position)
    {
        if (_visualRoot != null)
        {
            _visualRoot.SetActive(false);
        }

        if (_effectPool != null)
        {
            _effectPool.Get().PlayAt(position);
        }
    }
}