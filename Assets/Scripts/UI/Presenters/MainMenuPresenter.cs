using System;

public class MainMenuPresenter : IDisposable
{
    private readonly MainMenuScreen _screen;

    public event Action StartRequested;

    public MainMenuPresenter(MainMenuScreen screen)
    {
        _screen = screen;
        _screen.StartClicked += OnStartClicked;
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
        _screen.StartClicked -= OnStartClicked;
    }

    private void OnStartClicked()
    {
        StartRequested?.Invoke();
    }
}