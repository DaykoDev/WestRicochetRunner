using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [Serializable]
    private class UpgradePurchaseView
    {
        [SerializeField] private PermanentUpgradeType _type;
        [SerializeField] private Button _button;
        [SerializeField] private LocalizedText _levelText;
        [SerializeField] private LocalizedText _costText;
        [SerializeField] private GameObject _root;

        public PermanentUpgradeType Type => _type;
        public Button Button => _button;
        public LocalizedText LevelText => _levelText;
        public LocalizedText CostText => _costText;
        public GameObject Root => _root;

        public event Action<PermanentUpgradeType> PurchaseRequested;

        public void HandleButtonClicked()
        {
            PurchaseRequested?.Invoke(_type);
        }
    }

    [Header("Progress")]
    [SerializeField] private LocalizedText _levelText;

    [Header("Coins")]
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private LocalizedText _coinsRewardText;
    [SerializeField, Range(0f, 1f)] private float _coinsPunchStrength = 0.2f;
    [SerializeField, Min(0.02f)] private float _coinsPunchDuration = 0.18f;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _coinsRewardButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _leaderboardButton;

    [Header("Permanent Upgrades")]
    [SerializeField] private UpgradePurchaseView[] _upgradeViews;

    private readonly Dictionary<PermanentUpgradeType, UpgradePurchaseView> _views = new();

    private int _previousCoins;
    private bool _isCoinsInitialized;

    public event Action StartClicked;
    public event Action CoinsRewardClicked;
    public event Action SettingsClicked;
    public event Action LeaderboardClicked;
    public event Action<PermanentUpgradeType> UpgradePurchaseRequested;

    private void Awake()
    {
        if (_upgradeViews == null)
        {
            return;
        }

        foreach (UpgradePurchaseView view in _upgradeViews)
        {
            if (view == null)
            {
                continue;
            }

            if (_views.ContainsKey(view.Type))
            {
                Debug.LogWarning($"Duplicate upgrade view: {view.Type}", this);
                continue;
            }

            _views.Add(view.Type, view);
        }
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartClicked);
        _coinsRewardButton.onClick.AddListener(OnCoinsRewardClicked);
        _settingsButton.onClick.AddListener(OnSettingsClicked);
        _leaderboardButton.onClick.AddListener(OnLeaderboardClicked);

        foreach (UpgradePurchaseView view in _views.Values)
        {
            if (view.Button != null)
            {
                view.Button.onClick.AddListener(view.HandleButtonClicked);
                view.PurchaseRequested += OnUpgradePurchaseRequested;
            }
        }
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartClicked);
        _coinsRewardButton.onClick.RemoveListener(OnCoinsRewardClicked);
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        _leaderboardButton.onClick.RemoveListener(OnLeaderboardClicked);

        foreach (UpgradePurchaseView view in _views.Values)
        {
            if (view.Button != null)
            {
                view.Button.onClick.RemoveListener(view.HandleButtonClicked);
                view.PurchaseRequested -= OnUpgradePurchaseRequested;
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetLevel(int level)
    {
        if (_levelText != null)
        {
            _levelText.SetKey(LocalizationKeys.Level, level);
        }
    }

    public void SetCoins(int coins)
    {
        if (_coinsText == null)
        {
            return;
        }

        bool shouldAnimate = _isCoinsInitialized && coins != _previousCoins;

        _coinsText.text = coins.ToString();

        _previousCoins = coins;
        _isCoinsInitialized = true;

        if (shouldAnimate == false)
        {
            return;
        }

        Transform target = _coinsText.transform;

        target.DOKill();
        target.localScale = Vector3.one;

        target.DOPunchScale(Vector3.one * _coinsPunchStrength, _coinsPunchDuration);
    }

    public void SetCoinsReward(int reward, bool isAvailable)
    {
        _coinsRewardButton.interactable = isAvailable;

        if (_coinsRewardText != null)
        {
            _coinsRewardText.SetKey(LocalizationKeys.CoinsReward, reward);
        }
    }

    public void SetUpgradeInfo(PermanentUpgradeType type, int level, int maxLevel, int nextCost, bool canPurchase)
    {
        if (_views.TryGetValue(type, out UpgradePurchaseView view) == false)
        {
            return;
        }

        if (view.LevelText != null)
        {
            if (maxLevel < 0)
            {
                view.LevelText.SetKey(LocalizationKeys.Unavailable);
            }
            else if (maxLevel == 0)
            {
                view.LevelText.SetPlainText(level.ToString());
            }
            else
            {
                view.LevelText.SetPlainText($"{level}/{maxLevel}");
            }
        }

        bool isMaxLevel = maxLevel > 0 && level >= maxLevel;

        if (view.CostText != null)
        {
            if (maxLevel < 0)
            {
                view.CostText.SetKey(LocalizationKeys.Unavailable);
            }
            else if (isMaxLevel)
            {
                view.CostText.SetKey(LocalizationKeys.Max);
            }
            else
            {
                view.CostText.SetPlainText(nextCost.ToString());
            }
        }

        if (view.Button != null)
        {
            view.Button.interactable = canPurchase;
        }
    }

    public void SetUpgradeUnlocked(PermanentUpgradeType type, bool isUnlocked)
    {
        if (_views.TryGetValue(type, out UpgradePurchaseView view) == false)
        {
            return;
        }

        if (view.Root != null)
        {
            view.Root.SetActive(isUnlocked);
        }
    }

    private void OnUpgradePurchaseRequested(PermanentUpgradeType type)
    {
        UpgradePurchaseRequested?.Invoke(type);
    }

    private void OnStartClicked()
    {
        StartClicked?.Invoke();
    }

    private void OnCoinsRewardClicked()
    {
        CoinsRewardClicked?.Invoke();
    }

    private void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
    }

    private void OnLeaderboardClicked()
    {
        LeaderboardClicked?.Invoke();
    }
}