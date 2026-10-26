using System;

public sealed class GameFlow : IDisposable
{
    private readonly GameStateMachine _stateMachine;
    private readonly PlayerProgress _playerProgress;
    private readonly PlayerHealth _playerHealth;
    private readonly LevelGenerator _levelGenerator;
    private readonly RunState _runState;
    private readonly SecondChanceFlow _secondChanceFlow;
    private readonly AdsService _adsService;
    private readonly Wallet _wallet;
    private readonly RunRewardCollector _rewardCollector;

    private bool _isDisposed;

    public GameFlow(
        GameStateMachine stateMachine,
        PlayerProgress playerProgress,
        PlayerHealth playerHealth,
        LevelGenerator levelGenerator,
        RunState runState,
        SecondChanceFlow secondChanceFlow,
        AdsService adsService,
        Wallet wallet,
        RunRewardCollector rewardCollector)
    {
        _stateMachine = stateMachine;
        _playerProgress = playerProgress;
        _playerHealth = playerHealth;
        _levelGenerator = levelGenerator;
        _runState = runState;
        _secondChanceFlow = secondChanceFlow;
        _adsService = adsService;
        _wallet = wallet;
        _rewardCollector = rewardCollector;

        Subscribe();
    }

    public void EnterMainMenuFromBootstrap()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    public void StartRun()
    {
        _stateMachine.Enter<RunState>();
    }

    public void RestartRun()
    {
        _runState.ClearRun();
        _stateMachine.Enter<RunState>();
    }

    public void PauseRun()
    {
        _stateMachine.Enter<PausedState>();
    }

    public void ResumeRun()
    {
        _stateMachine.Enter<RunState, RunEnterMode>(RunEnterMode.Resume);
    }

    public void ReturnToMainMenu()
    {
        _adsService.TryShowInterstitial();

        _runState.ClearRun();
        _stateMachine.Enter<MainMenuState>();
    }

    public void AbortRun()
    {
        _wallet.Remove(_rewardCollector.CollectedThisRun);
        ReturnToMainMenu();
    }

    public void CompleteVictoryBonus(int bonus)
    {
        ReturnToMainMenu();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Unsubscribe();
    }

    private void Subscribe()
    {
        _playerHealth.Died += OnPlayerDied;
        _levelGenerator.Finished += OnLevelFinished;

        _runState.Won += OnRunWon;
        _runState.Lost += OnRunLost;
        _runState.Failed += OnRunFailed;

        _secondChanceFlow.SecondChanceGranted += OnSecondChanceGranted;
    }

    private void Unsubscribe()
    {
        _playerHealth.Died -= OnPlayerDied;
        _levelGenerator.Finished -= OnLevelFinished;

        _runState.Won -= OnRunWon;
        _runState.Lost -= OnRunLost;
        _runState.Failed -= OnRunFailed;

        _secondChanceFlow.SecondChanceGranted -= OnSecondChanceGranted;
    }

    private void OnPlayerDied()
    {
        _runState.RequestLose();
    }

    private void OnLevelFinished()
    {
        _runState.RequestWin();
    }

    private void OnRunWon()
    {
        _rewardCollector.GrantCompletionReward(_playerProgress.CurrentLevel);
        _playerProgress.CompleteCurrentLevel();
        _stateMachine.Enter<VictoryState>();
    }

    private void OnRunLost()
    {
        _stateMachine.Enter<DefeatState>();
    }

    private void OnRunFailed()
    {
        _runState.ClearRun();
        _stateMachine.Enter<MainMenuState>();
    }

    private void OnSecondChanceGranted()
    {
        ResumeRun();
    }
}