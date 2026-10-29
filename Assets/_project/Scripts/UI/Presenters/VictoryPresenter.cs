using System;

public class VictoryPresenter : IDisposable
{
    private readonly VictoryScreen _screen;
    private readonly RunRewardCollector _rewardCollector;
    private readonly RewardedMultiplierFlow _multiplierFlow;
    private readonly Wallet _wallet;

    public VictoryPresenter(VictoryScreen screen, RunRewardCollector rewardCollector, RewardedMultiplierFlow rewardedMultiplierFlow, Wallet wallet)
    {
        _screen = screen;
        _rewardCollector = rewardCollector;
        _multiplierFlow = rewardedMultiplierFlow;
        _wallet = wallet;

        _screen.MainMenuClicked += OnMainMenuClicked;
        _screen.MultiplyClicked += OnMultiplyClicked;
        _multiplierFlow.BonusGranted += OnBonusGranted;

        _multiplierFlow.AvailabilityChanged += OnMultiplierAvailabilityChanged;

        _wallet.BalanceChanged += OnWalletBalanceChanged;
    }

    public event Action MainMenuRequested;

    public void HandleStateEntered()
    {
        _screen.SetCollectedCoins(_rewardCollector.CollectedThisRun);
        _screen.SetWalletCoins(_wallet.Balance);

        _multiplierFlow.Reset();

        _screen.Show();
        OnMultiplierAvailabilityChanged();
    }

    public void HandleStateExited()
    {
        _multiplierFlow.Cancel();
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.MainMenuClicked -= OnMainMenuClicked;
        _screen.MultiplyClicked -= OnMultiplyClicked;
        _multiplierFlow.BonusGranted -= OnBonusGranted;

        _multiplierFlow.AvailabilityChanged -= OnMultiplierAvailabilityChanged;

        _wallet.BalanceChanged -= OnWalletBalanceChanged;
    }

    private void OnMainMenuClicked()
    {
        MainMenuRequested?.Invoke();
    }

    private void OnMultiplyClicked()
    {
        _multiplierFlow.Request();
    }

    private void OnBonusGranted(int bonus)
    {
        _screen.SetCollectedCoins(_rewardCollector.CollectedThisRun + bonus);
        _screen.SetMultiplyAvailable(false);
    }

    private void OnMultiplierAvailabilityChanged()
    {
        _screen.SetMultiplyAvailable(_multiplierFlow.CanApply);
    }

    private void OnWalletBalanceChanged(int balance)
    {
        _screen.SetWalletCoins(balance);
    }
}