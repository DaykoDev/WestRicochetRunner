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
        private bool _isScaleCaptured;
        private Tween _punchTween;
        private Vector3 _initialScale;

        private void Awake()
        {
            if (_text == null)
            {
                Debug.LogError("PunchTextView: Text is not assigned.", this);
                enabled = false;
                return;
            }

            CaptureInitialScale();
        }

        private void OnDisable()
        {
            _punchTween?.Kill();

            if (_text != null)
            {
                _text.transform.localScale = _initialScale;
            }
        }

        public void SetText(string text)
        {
            if (_text == null)
            {
                return;
            }

            CaptureInitialScale();

            bool shouldAnimate = _isInitialized && text != _previousText;

            _text.text = text;
            _previousText = text;
            _isInitialized = true;

            if (shouldAnimate == false)
            {
                return;
            }

            PlayPunch();
        }

        private void CaptureInitialScale()
        {
            if (_isScaleCaptured)
            {
                return;
            }

            _initialScale = _text.transform.localScale;
            _isScaleCaptured = true;
        }

        private void PlayPunch()
        {
            Transform target = _text.transform;

            _punchTween?.Kill();
            target.localScale = _initialScale;

            _punchTween = target
                .DOPunchScale(Vector3.up * _punchStrength, _punchDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    target.localScale = _initialScale;
                    _punchTween = null;
                });
        }
    }
}