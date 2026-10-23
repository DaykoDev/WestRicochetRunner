using System;

public class DefeatPresenter : IDisposable
{
    private readonly DefeatScreen _screen;
    private readonly SecondChanceFlow _secondChanceFlow;

    public DefeatPresenter(DefeatScreen screen, SecondChanceFlow secondChanceFlow)
    {
        _screen = screen;
        _secondChanceFlow = secondChanceFlow;

        _screen.MainMenuClicked += OnMainMenuClicked;
        _screen.RestartClicked += OnRestartClicked;
        _screen.SecondChanceClicked += OnSecondChanceClicked;
    }

    public event Action MainMenuRequested;
    public event Action RestartRequested;

    public void HandleStateEntered()
    {
        _screen.Show();
        _screen.SetSecondChanceAvailable(_secondChanceFlow.CanOffer);
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.MainMenuClicked -= OnMainMenuClicked;
        _screen.RestartClicked -= OnRestartClicked;
        _screen.SecondChanceClicked -= OnSecondChanceClicked;
    }

    private void OnMainMenuClicked()
    {
        MainMenuRequested?.Invoke();
    }

    private void OnRestartClicked()
    {
        RestartRequested?.Invoke();
    }

    private void OnSecondChanceClicked()
    {
        _secondChanceFlow.Request();
    }
}