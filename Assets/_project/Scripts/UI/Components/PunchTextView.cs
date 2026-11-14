using DG.Tweening;
using TMPro;
using UnityEngine;

namespace WestRicochet.UI.Components
{
    public sealed class PunchTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        [SerializeField, Range(0f, 1f)] private float _punchStrength = 0.2f;

        [SerializeField, Min(0.02f)] private float _punchDuration = 0.18f;

        private string _previousText;
        private bool _isInitialized;
        private Tween _punchTween;

        private void OnDisable()
        {
            _punchTween?.Kill();
            _punchTween = null;

            if (_text != null)
            {
                _text.transform.localScale = Vector3.one;
            }
        }

        public void SetText(string text)
        {
            if (_text == null)
            {
                return;
            }

            bool shouldAnimate =
                _isInitialized && text != _previousText;

            _text.text = text;

            _previousText = text;
            _isInitialized = true;

            if (shouldAnimate == false)
            {
                return;
            }

            PlayPunch();
        }

        private void PlayPunch()
        {
            Transform target = _text.transform;

            _punchTween?.Kill();
            target.localScale = Vector3.one;

            _punchTween = target
                .DOPunchScale(
                    Vector3.up * _punchStrength,
                    _punchDuration)
                .OnComplete(() => _punchTween = null);
        }
    }
}