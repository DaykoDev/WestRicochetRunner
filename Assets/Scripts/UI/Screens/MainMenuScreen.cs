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
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _costText;

        public PermanentUpgradeType Type => _type;
        public Button Button => _button;
        public TMP_Text LevelText => _levelText;
        public TMP_Text CostText => _costText;

        public event Action<PermanentUpgradeType> PurchaseRequested;

        public void HandleButtonClicked()
        {
            PurchaseRequested?.Invoke(_type);
        }
    }

    [Header("Progress")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _coinsText;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _coinsRewardButton;
    [SerializeField] private Button _settingsButton;

    [Header("Permanent Upgrades")]
    [SerializeField] private UpgradePurchaseView[] _upgradeViews;

    private readonly Dictionary<PermanentUpgradeType, UpgradePurchaseView> _views = new();

    public event Action StartClicked;
    public event Action CoinsRewardClicked;
    public event Action SettingsClicked;
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
            _levelText.text = $"Level: {level}";
        }
    }

    public void SetCoins(int coins)
    {
        if (_coinsText != null)
        {
            _coinsText.text = $"Coins: {coins}";
        }
    }

    public void SetCoinsRewardAvailable(bool isAvailable)
    {
        _coinsRewardButton.interactable = isAvailable;
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
                view.LevelText.text = "Unavailable";
            }
            else if (maxLevel == 0)
            {
                view.LevelText.text = $"Level: {level}";
            }
            else
            {
                view.LevelText.text = $"Level: {level}/{maxLevel}";
            }
        }

        bool isMaxLevel = maxLevel > 0 && level >= maxLevel;

        if (view.CostText != null)
        {
            if (maxLevel < 0)
            {
                view.CostText.text = "Unavailable";
            }
            else if (isMaxLevel)
            {
                view.CostText.text = "MAX";
            }
            else
            {
                view.CostText.text = $"Cost: {nextCost}";
            }
        }

        if (view.Button != null)
        {
            view.Button.interactable = canPurchase;
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
}