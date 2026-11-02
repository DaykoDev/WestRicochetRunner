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
    private bool _isFinalBonusActive;

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

    public void EnterInitialState()
    {
        if (_playerProgress.IsTutorialCompleted == false)
        {
            _stateMachine.Enter<RunState>();
            return;
        }

        _stateMachine.Enter<MainMenuState>();
    }

    public void StartRun()
    {
        ResetFinalBonus();
        _stateMachine.Enter<RunState>();
    }

    public void RestartRun()
    {
        ResetFinalBonus();
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
        ResetFinalBonus();
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
        _levelGenerator.FinalBonusCompleted += OnFinalBonusCompleted;
    }

    private void Unsubscribe()
    {
        _playerHealth.Died -= OnPlayerDied;
        _levelGenerator.Finished -= OnLevelFinished;

        _runState.Won -= OnRunWon;
        _runState.Lost -= OnRunLost;
        _runState.Failed -= OnRunFailed;

        _secondChanceFlow.SecondChanceGranted -= OnSecondChanceGranted;
        _levelGenerator.FinalBonusCompleted -= OnFinalBonusCompleted;
    }

    private void OnPlayerDied()
    {
        if (_isFinalBonusActive)
        {
            _runState.RequestWin();
            return;
        }

        _runState.RequestLose();
    }

    private void OnLevelFinished()
    {
        if (_isFinalBonusActive)
        {
            return;
        }

        _isFinalBonusActive = true;
        _playerHealth.SetOneHitDeathEnabled(true);
    }

    private void OnFinalBonusCompleted()
    {
        if (_isFinalBonusActive == false)
        {
            return;
        }

        _runState.RequestWin();
    }

    private void OnRunWon()
    {
        ResetFinalBonus();
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

    private void ResetFinalBonus()
    {
        _isFinalBonusActive = false;
        _playerHealth.SetOneHitDeathEnabled(false);
    }
}