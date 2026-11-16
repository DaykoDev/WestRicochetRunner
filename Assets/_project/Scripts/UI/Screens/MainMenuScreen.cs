using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WestRicochet.Infrastructure.Localization;
using WestRicochet.Meta;
using WestRicochet.UI.Components;

namespace WestRicochet.UI.Screens
{
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("Progress")]
        [SerializeField] private LocalizedText _levelText;

        [Header("Coins")]
        [SerializeField] private LocalizedText _coinsRewardText;
        [SerializeField] private PunchTextView _coinsView;

        [Header("Buttons")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _coinsRewardButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leaderboardButton;

        [Header("Permanent Upgrades")]
        [SerializeField] private UpgradePurchaseView[] _upgradeViews;

        private readonly Dictionary<PermanentUpgradeType, UpgradePurchaseView> _views = new();

        public event Action StartClicked;
        public event Action CoinsRewardClicked;
        public event Action SettingsClicked;
        public event Action LeaderboardClicked;
        public event Action<PermanentUpgradeType> UpgradePurchaseRequested;

        private void Awake()
        {
            if (_startButton == null ||
                _coinsRewardButton == null ||
                _settingsButton == null ||
                _leaderboardButton == null)
            {
                Debug.LogError(
                    "MainMenuScreen: All buttons must be assigned.",
                    this);

                enabled = false;
                return;
            }

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
            _startButton?.onClick.RemoveListener(OnStartClicked);
            _coinsRewardButton?.onClick.RemoveListener(OnCoinsRewardClicked);
            _settingsButton?.onClick.RemoveListener(OnSettingsClicked);
            _leaderboardButton?.onClick.RemoveListener(OnLeaderboardClicked);

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
            if (_coinsView != null)
            {
                _coinsView.SetText(coins.ToString());
            }
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
}