using System;

public class VictoryPresenter : IDisposable
{
    private readonly VictoryScreen _screen;
    private readonly RunRewardCollector _rewardCollector;
    private readonly RewardedMultiplierFlow _multiplierFlow;

    public VictoryPresenter(VictoryScreen screen, RunRewardCollector rewardCollector, RewardedMultiplierFlow rewardedMultiplierFlow)
    {
        _screen = screen;
        _rewardCollector = rewardCollector;
        _multiplierFlow = rewardedMultiplierFlow;

        _screen.MainMenuClicked += OnMainMenuClicked;
        _screen.MultiplyClicked += OnMultiplyClicked;
        _multiplierFlow.BonusGranted += OnBonusGranted;

        _multiplierFlow.AvailabilityChanged += OnMultiplierAvailabilityChanged;
    }

    public event Action MainMenuRequested;

    public void HandleStateEntered()
    {
        _screen.SetCollectedCoins(_rewardCollector.CollectedThisRun);
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
}