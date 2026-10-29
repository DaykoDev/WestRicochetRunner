using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _multiplyButton;
    [SerializeField, Min(0f)] private float _mainMenuButtonDelay = 2f;

    [Header("Reward")]
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField, Min(0f)] private float _coinsCountDuration = 0.6f;

    [Header("Wallet")]
    [SerializeField] private TMP_Text _walletCoinsText;

    private int _displayedCoins;
    private Tween _coinsTween;
    private Tween _mainMenuDelayTween;

    public event Action MainMenuClicked;
    public event Action MultiplyClicked;

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        _multiplyButton.onClick.AddListener(OnMultiplyClicked);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        _multiplyButton.onClick.RemoveListener(OnMultiplyClicked);

        _coinsTween?.Kill();

        _mainMenuDelayTween?.Kill();
        _mainMenuDelayTween = null;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _mainMenuDelayTween?.Kill();

        _mainMenuButton.gameObject.SetActive(false);

        _mainMenuDelayTween = DOVirtual.DelayedCall(
                _mainMenuButtonDelay,
                () =>
                {
                    _mainMenuButton.gameObject.SetActive(true);
                })
            .SetUpdate(true);
    }

    public void SetWalletCoins(int coins)
    {
        if (_walletCoinsText != null)
        {
            _walletCoinsText.text = coins.ToString();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetMultiplyAvailable(bool isAvailable)
    {
        _multiplyButton.interactable = isAvailable;
    }

    private void OnMultiplyClicked()
    {
        MultiplyClicked?.Invoke();
    }

    public void SetCollectedCoins(int coins)
    {
        _coinsTween?.Kill();
        _coinsTween = DOTween.To(
                () => _displayedCoins,
                value =>
                {
                    _displayedCoins = value;
                    _coinsText.text = $"+{value}";
                },
                coins,
                _coinsCountDuration)
            .SetUpdate(true);
    }

    private void OnMainMenuClicked()
    {
        MainMenuClicked?.Invoke();
    }
}