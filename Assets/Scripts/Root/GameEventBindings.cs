using System;

public sealed class GameEventBindings : IDisposable
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly MetaComposition _meta;
    private readonly RunComposition _run;
    private readonly UiComposition _ui;
    private readonly SettingsFlow _settingsFlow;

    private bool _isDisposed;

    public GameEventBindings(ILeaderboardService leaderboardService, MetaComposition meta, RunComposition run, UiComposition ui, SettingsFlow settingsFlow)
    {
        _leaderboardService = leaderboardService;
        _meta = meta;
        _run = run;
        _ui = ui;
        _settingsFlow = settingsFlow;

        SubscribeToStateEvents();
        SubscribeToPresenterEvents();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        UnsubscribeFromPresenterEvents();
        UnsubscribeFromStateEvents();
    }

    private void SubscribeToStateEvents()
    {
        _run.BootstrapState.Completed += OnBootstrapCompleted;

        _run.MainMenuState.Entered += _ui.MainMenu.HandleStateEntered;

        _run.MainMenuState.Exited += _ui.MainMenu.HandleStateExited;

        _run.RunState.Entered += _ui.Run.HandleStateEntered;

        _run.RunState.Entered += OnRunEntered;

        _run.RunState.Exited += _ui.Run.HandleStateExited;

        _run.VictoryState.Entered += _ui.Victory.HandleStateEntered;

        _run.VictoryState.Exited += _ui.Victory.HandleStateExited;

        _run.DefeatState.Entered += _ui.Defeat.HandleStateEntered;

        _run.DefeatState.Exited += _ui.Defeat.HandleStateExited;

        _meta.Progress.BestLevelChanged += _leaderboardService.SubmitScore;
    }

    private void UnsubscribeFromStateEvents()
    {
        _run.BootstrapState.Completed -= OnBootstrapCompleted;

        _run.MainMenuState.Entered -= _ui.MainMenu.HandleStateEntered;

        _run.MainMenuState.Exited -= _ui.MainMenu.HandleStateExited;

        _run.RunState.Entered -= _ui.Run.HandleStateEntered;

        _run.RunState.Entered -= OnRunEntered;

        _run.RunState.Exited -= _ui.Run.HandleStateExited;

        _run.VictoryState.Entered -= _ui.Victory.HandleStateEntered;

        _run.VictoryState.Exited -= _ui.Victory.HandleStateExited;

        _run.DefeatState.Entered -= _ui.Defeat.HandleStateEntered;

        _run.DefeatState.Exited -= _ui.Defeat.HandleStateExited;

        _meta.Progress.BestLevelChanged -= _leaderboardService.SubmitScore;
    }

    private void SubscribeToPresenterEvents()
    {
        _ui.MainMenu.StartRequested += _run.GameFlow.StartRun;

        _ui.Victory.MainMenuRequested += _run.GameFlow.ReturnToMainMenu;

        _ui.Defeat.MainMenuRequested += _run.GameFlow.ReturnToMainMenu;

        _ui.Defeat.RestartRequested += _run.GameFlow.RestartRun;

        _run.VictoryMultiplier.BonusGranted += _run.GameFlow.CompleteVictoryBonus;

        _ui.Run.SettingsRequested += _settingsFlow.OpenFromRun;

        _ui.MainMenu.SettingsRequested += _settingsFlow.OpenFromMenu;
    }

    private void UnsubscribeFromPresenterEvents()
    {
        _ui.MainMenu.StartRequested -= _run.GameFlow.StartRun;

        _ui.Victory.MainMenuRequested -= _run.GameFlow.ReturnToMainMenu;

        _ui.Defeat.MainMenuRequested -= _run.GameFlow.ReturnToMainMenu;

        _ui.Defeat.RestartRequested -= _run.GameFlow.RestartRun;

        _run.VictoryMultiplier.BonusGranted -= _run.GameFlow.CompleteVictoryBonus;

        _ui.Run.SettingsRequested -= _settingsFlow.OpenFromRun;

        _ui.MainMenu.SettingsRequested -= _settingsFlow.OpenFromMenu;
    }

    private void OnBootstrapCompleted()
    {
        _run.GameFlow.EnterMainMenuFromBootstrap();
    }

    private void OnRunEntered()
    {
        _run.SecondChance.SetContext(_run.RunState.Context);
    }
}