using System;

public sealed class SettingsFlow : IDisposable
{
    private readonly SettingsPresenter _settingsPresenter;
    private readonly PausedState _pausedState;
    private readonly GameFlow _gameFlow;

    private bool _isRunPause;
    private bool _isDisposed;

    public SettingsFlow(
        SettingsPresenter settingsPresenter,
        PausedState pausedState,
        GameFlow gameFlow)
    {
        _settingsPresenter = settingsPresenter;
        _pausedState = pausedState;
        _gameFlow = gameFlow;

        _pausedState.Entered += OnPausedStateEntered;
        _pausedState.Exited += OnPausedStateExited;

        _settingsPresenter.CloseRequested += OnCloseRequested;
        _settingsPresenter.ExitToMenuRequested += OnExitToMenuRequested;
    }

    public void OpenFromMenu()
    {
        _isRunPause = false;
        _settingsPresenter.Show(false);
    }

    public void OpenFromRun()
    {
        _isRunPause = true;
        _gameFlow.PauseRun();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _pausedState.Entered -= OnPausedStateEntered;
        _pausedState.Exited -= OnPausedStateExited;

        _settingsPresenter.CloseRequested -= OnCloseRequested;
        _settingsPresenter.ExitToMenuRequested -= OnExitToMenuRequested;
    }

    private void OnPausedStateEntered()
    {
        _settingsPresenter.Show(true);
    }

    private void OnPausedStateExited()
    {
        _isRunPause = false;
        _settingsPresenter.Hide();
    }

    private void OnCloseRequested()
    {
        if (_isRunPause)
        {
            _gameFlow.ResumeRun();
        }
        else
        {
            _settingsPresenter.Hide();
        }
    }

    private void OnExitToMenuRequested()
    {
        _isRunPause = false;
        _gameFlow.AbortRun();
    }
}