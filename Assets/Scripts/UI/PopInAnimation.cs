using DG.Tweening;
using UnityEngine;

public class PopInAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _target;
    [SerializeField, Min(0f)] private float _duration = 0.25f;
    [SerializeField, Min(0f)] private float _startScale = 0.8f;
    [SerializeField] private Ease _ease = Ease.OutBack;

    private void Awake()
    {
        if (_target == null)
        {
            _target = transform as RectTransform;
        }
    }

    private void OnEnable()
    {
        _target.localScale = Vector3.one * _startScale;
        _target.DOScale(Vector3.one, _duration).SetEase(_ease).SetUpdate(true);
    }

    private void OnDisable()
    {
        _target.DOKill();
    }
}