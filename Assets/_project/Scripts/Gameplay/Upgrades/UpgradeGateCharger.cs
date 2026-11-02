using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeGateCharger : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private UpgradeType _type = UpgradeType.BallCount;
    [SerializeField, Min(0f)] private float _initialBonus = 2f;
    [SerializeField, Min(0f)] private float _bonusPerBall = 1f;
    [SerializeField, Min(0f)] private float _maxBonus = 15f;

    [Header("Display")]
    [SerializeField] private TMP_Text _bonusText;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float _countDuration = 0.12f;
    [SerializeField, Min(0f)] private float _punchDuration = 0.18f;
    [SerializeField, Min(0f)] private float _punchScale = 0.25f;

    private readonly Dictionary<Ball, int> _lastChargedLaunches = new();

    private Tween _countTween;
    private Tween _punchTween;

    private Vector3 _initialTextScale;
    private float _displayedBonus;

    public UpgradeType Type => _type;
    public float CurrentBonus { get; private set; }

    private void Awake()
    {
        CurrentBonus = Mathf.Clamp(_initialBonus, 0f, _maxBonus);

        _displayedBonus = CurrentBonus;

        if (_bonusText != null)
        {
            _initialTextScale = _bonusText.transform.localScale;
        }

        UpdateText(CurrentBonus);
    }

    public void TryCharge(Ball ball)
    {
        if (ball == null || ball.IsActive == false)
        {
            return;
        }

        if (_lastChargedLaunches.TryGetValue(ball, out int launchId) && launchId == ball.LaunchId)
        {
            return;
        }

        _lastChargedLaunches[ball] = ball.LaunchId;

        float newBonus = Mathf.Min(_maxBonus, CurrentBonus + _bonusPerBall);

        if (Mathf.Approximately(newBonus, CurrentBonus))
        {
            return;
        }

        CurrentBonus = newBonus;
        AnimateDisplay();
    }

    private void AnimateDisplay()
    {
        if (_bonusText == null)
        {
            _displayedBonus = CurrentBonus;
            return;
        }

        _countTween?.Kill();
        _punchTween?.Kill();

        Transform textTransform = _bonusText.transform;

        textTransform.localScale = _initialTextScale;

        _countTween = DOVirtual.Float(
                _displayedBonus,
                CurrentBonus,
                _countDuration,
                value =>
                {
                    _displayedBonus = value;
                    UpdateText(value);
                })
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _displayedBonus = CurrentBonus;
                UpdateText(CurrentBonus);
                _countTween = null;
            });

        _punchTween = textTransform
            .DOPunchScale(
                Vector3.one * _punchScale,
                _punchDuration,
                1,
                0f)
            .OnComplete(() =>
            {
                textTransform.localScale = _initialTextScale;
                _punchTween = null;
            });
    }

    private void UpdateText(float value)
    {
        if (_bonusText == null)
        {
            return;
        }

        if (_type == UpgradeType.FireRate || _type == UpgradeType.BallSpeed)
        {
            _bonusText.text = $"+{value:0.#}%";
        }
        else
        {
            _bonusText.text = $"+{Mathf.RoundToInt(value)}";
        }
    }

    private void OnDisable()
    {
        _countTween?.Kill();
        _punchTween?.Kill();

        _countTween = null;
        _punchTween = null;

        if (_bonusText != null)
        {
            _bonusText.transform.localScale = _initialTextScale;
        }
    }
}