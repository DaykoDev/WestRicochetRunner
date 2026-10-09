using System;

public class RunPresenter : IDisposable
{
    private readonly RunScreen _screen;

    public event Action TestWinRequested;
    public event Action TestLoseRequested;

    public RunPresenter(RunScreen screen)
    {
        _screen = screen;

        _screen.TestWinClicked += OnTestWinClicked;
        _screen.TestLoseClicked += OnTestLoseClicked;
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
        _screen.TestWinClicked -= OnTestWinClicked;
        _screen.TestLoseClicked -= OnTestLoseClicked;
    }

    private void OnTestWinClicked()
    {
        TestWinRequested?.Invoke();
    }

    private void OnTestLoseClicked()
    {
        TestLoseRequested?.Invoke();
    }
}