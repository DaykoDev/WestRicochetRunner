using System;

public class VictoryPresenter : IDisposable
{
    private readonly VictoryScreen _screen;

    public event Action MainMenuRequested;

    public VictoryPresenter(VictoryScreen screen)
    {
        _screen = screen;
        _screen.MainMenuClicked += OnMainMenuClicked;
    }

    public void HandleStateEntered()
    {
        _screen.Show();
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.MainMenuClicked -= OnMainMenuClicked;
    }

    private void OnMainMenuClicked()
    {
        MainMenuRequested?.Invoke();
    }
}