using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        [NonSerialized] public UnityAction ClickHandler;
    }

    [Header("Progress")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _coinsText;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;

    [Header("Permanent Upgrades")]
    [SerializeField] private UpgradePurchaseView[] _upgradeViews;

    private readonly Dictionary<PermanentUpgradeType, UpgradePurchaseView> _views = new();

    public event Action StartClicked;
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
            view.ClickHandler = () => UpgradePurchaseRequested?.Invoke(view.Type);
        }
    }

    private void OnEnable()
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        foreach (UpgradePurchaseView view in _views.Values)
        {
            if (view.Button != null && view.ClickHandler != null)
            {
                view.Button.onClick.AddListener(view.ClickHandler);
            }
        }
    }

    private void OnDisable()
    {
        if (_startButton != null)
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        foreach (UpgradePurchaseView view in _views.Values)
        {
            if (view.Button != null && view.ClickHandler != null)
            {
                view.Button.onClick.RemoveListener(view.ClickHandler);
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

    private void OnStartButtonClicked()
    {
        StartClicked?.Invoke();
    }
}