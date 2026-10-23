using System;

public class MainMenuPresenter : IDisposable
{
    private readonly MainMenuScreen _screen;
    private readonly UpgradeService _upgradeService;
    private readonly RewardedCoinsFlow _coinsFlow;

    public MainMenuPresenter(MainMenuScreen screen, UpgradeService upgradeService, RewardedCoinsFlow coinsFlow)
    {
        _screen = screen;
        _upgradeService = upgradeService;
        _coinsFlow = coinsFlow;

        _screen.StartClicked += OnStartClicked;
        _screen.UpgradePurchaseRequested += OnUpgradePurchaseRequested;
        _screen.CoinsRewardClicked += OnCoinsRewardClicked;
        _screen.SettingsClicked += OnSettingsClicked;

        _upgradeService.LevelChanged += OnLevelChanged;
        _upgradeService.BalanceChanged += OnBalanceChanged;
        _upgradeService.UpgradeLevelChanged += OnUpgradeLevelChanged;
    }

    public event Action StartRequested;
    public event Action SettingsRequested;

    public void HandleStateEntered()
    {
        _screen.Show();
        _screen.SetLevel(_upgradeService.CurrentLevel);
        _screen.SetCoins(_upgradeService.Balance);
        RefreshAllUpgradeViews();
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.StartClicked -= OnStartClicked;
        _screen.UpgradePurchaseRequested -= OnUpgradePurchaseRequested;
        _screen.CoinsRewardClicked -= OnCoinsRewardClicked;
        _screen.SettingsClicked -= OnSettingsClicked;

        _upgradeService.LevelChanged -= OnLevelChanged;
        _upgradeService.BalanceChanged -= OnBalanceChanged;
        _upgradeService.UpgradeLevelChanged -= OnUpgradeLevelChanged;
    }

    private void RefreshAllUpgradeViews()
    {
        RefreshUpgradeView(PermanentUpgradeType.StartBallCount);
        RefreshUpgradeView(PermanentUpgradeType.BallDamage);
        RefreshUpgradeView(PermanentUpgradeType.IncomeMultiplier);
    }

    private void RefreshUpgradeView(PermanentUpgradeType type)
    {
        int level = _upgradeService.GetLevel(type);
        int maxLevel = _upgradeService.GetMaxLevel(type);
        int nextCost = _upgradeService.GetNextCost(type);
        bool canPurchase = _upgradeService.CanPurchase(type);

        _screen.SetUpgradeInfo(type, level, maxLevel, nextCost, canPurchase);
    }

    private void OnUpgradePurchaseRequested(PermanentUpgradeType type)
    {
        _upgradeService.TryPurchase(type);
    }

    private void OnStartClicked()
    {
        StartRequested?.Invoke();
    }

    private void OnCoinsRewardClicked()
    {
        _coinsFlow.Request();
    }

    private void OnSettingsClicked()
    {
        SettingsRequested?.Invoke();
    }

    private void OnLevelChanged(int level)
    {
        _screen.SetLevel(level);
    }

    private void OnUpgradeLevelChanged(PermanentUpgradeType type, int level)
    {
        RefreshUpgradeView(type);
    }

    private void OnBalanceChanged(int balance)
    {
        _screen.SetCoins(balance);
        RefreshAllUpgradeViews();
    }
}