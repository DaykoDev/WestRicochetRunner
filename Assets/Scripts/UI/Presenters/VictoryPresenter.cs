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
    }

    public event Action MainMenuRequested;

    public void HandleStateEntered()
    {
        _screen.SetCollectedCoins(_rewardCollector.CollectedThisRun);
        _multiplierFlow.Reset();
        _screen.SetMultiplyAvailable(true);
        _screen.Show();
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.MainMenuClicked -= OnMainMenuClicked;
        _screen.MultiplyClicked -= OnMultiplyClicked;
        _multiplierFlow.BonusGranted -= OnBonusGranted;
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
}