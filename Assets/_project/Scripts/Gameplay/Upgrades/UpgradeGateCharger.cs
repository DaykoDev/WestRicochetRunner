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

    private readonly Dictionary<Ball, int> _lastChargedLaunches = new();

    public UpgradeType Type => _type;
    public float CurrentBonus { get; private set; }

    private void Awake()
    {
        CurrentBonus = Mathf.Clamp(_initialBonus, 0f, _maxBonus);
        UpdateDisplay();
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
        CurrentBonus = Mathf.Min(_maxBonus, CurrentBonus + _bonusPerBall);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_bonusText == null)
        {
            return;
        }

        if (_type == UpgradeType.FireRate || _type == UpgradeType.BallSpeed)
        {
            _bonusText.text = $"+{CurrentBonus:0.#}%";
        }
        else
        {
            _bonusText.text = $"+{Mathf.RoundToInt(CurrentBonus)}";
        }
    }
}