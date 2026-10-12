using System;

public class MainMenuPresenter : IDisposable
{
    private readonly MainMenuScreen _screen;

    public MainMenuPresenter(MainMenuScreen screen)
    {
        _screen = screen;
        _screen.StartClicked += OnStartClicked;
    }

    public event Action StartRequested;

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
        _screen.StartClicked -= OnStartClicked;
    }

    private void OnStartClicked()
    {
        StartRequested?.Invoke();
    }
}