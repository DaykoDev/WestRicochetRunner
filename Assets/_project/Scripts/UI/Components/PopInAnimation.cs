using DG.Tweening;
using UnityEngine;

namespace WestRicochet.UI.Components
{
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
                Debug.LogError("PopInAnimation: Target is not assigned.", this);

                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_target == null || enabled == false)
            {
                return;
            }

            _target.DOKill();
            _target.localScale = Vector3.one * _startScale;

            _target
                .DOScale(Vector3.one, _duration)
                .SetEase(_ease)
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            if (_target == null || enabled == false)
            {
                return;
            }

            _target.DOKill();
        }
    }
}