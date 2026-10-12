using System;

public class DefeatPresenter : IDisposable
{
    private readonly DefeatScreen _screen;

    public DefeatPresenter(DefeatScreen screen)
    {
        _screen = screen;
        _screen.MainMenuClicked += OnMainMenuClicked;
    }

    public event Action MainMenuRequested;

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