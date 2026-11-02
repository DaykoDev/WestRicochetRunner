using DG.Tweening;
using UnityEngine;

public sealed class TutorialSwipeHint : MonoBehaviour
{
    [SerializeField] private RectTransform _hand;

    [SerializeField] private float _swipeDistance = 120f;

    [SerializeField, Min(0.1f)]
    private float _swipeDuration = 0.75f;

    [SerializeField, Min(0f)]
    private float _pauseDuration = 0.15f;

    private Vector2 _startPosition;
    private Tween _swipeTween;

    private void Awake()
    {
        _startPosition = _hand.anchoredPosition;
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        _swipeTween?.Kill();
        _swipeTween = null;

        if (_hand != null)
        {
            _hand.anchoredPosition = _startPosition;
        }
    }

    private void StartAnimation()
    {
        _swipeTween?.Kill();

        _hand.anchoredPosition = _startPosition;

        _swipeTween = DOTween.Sequence()
            .Append(
                _hand.DOAnchorPosX(
                        _startPosition.x + _swipeDistance,
                        _swipeDuration)
                    .SetEase(Ease.InOutSine))
            .AppendInterval(_pauseDuration)
            .Append(
                _hand.DOAnchorPosX(
                        _startPosition.x,
                        _swipeDuration)
                    .SetEase(Ease.InOutSine))
            .AppendInterval(_pauseDuration)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
    }
}