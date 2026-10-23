using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositionRoot
{
    private readonly SceneReferences _sceneReferences;

    private GameStateMachine _stateMachine;
    private PlayerProgress _playerProgress;
    private PlayerHealth _playerHealth;
    private Wallet _wallet;
    private SaveService _saveService;
    private UpgradeService _upgradeService;
    private SettingsService _settingsService;

    private AdsService _adsService;
    private IAdsService _adsProvider;
    private ILeaderboardService _leaderboardService;
    private SecondChanceFlow _secondChanceFlow;
    private RewardedMultiplierFlow _multiplierFlow;
    private RewardedCoinsFlow _coinsFlow;

    private BootstrapState _bootstrapState;
    private MainMenuState _mainMenuState;
    private RunState _runState;
    private VictoryState _victoryState;
    private DefeatState _defeatState;
    private PausedState _pausedState;

    private MainMenuPresenter _mainMenuPresenter;
    private RunPresenter _runPresenter;
    private VictoryPresenter _victoryPresenter;
    private DefeatPresenter _defeatPresenter;
    private SettingsPresenter _settingsPresenter;

    private bool _isInitializing;
    private bool _isInitialized;
    private bool _isDisposed;
    private bool _settingsPauseContext;

    public CompositionRoot(SceneReferences sceneReferences)
    {
        _sceneReferences = sceneReferences;
    }

    public void Initialize()
    {
        if (_isDisposed || _isInitialized || _isInitializing)
        {
            return;
        }

        if (_sceneReferences == null)
        {
            Debug.LogError("CompositionRoot: SceneReferences is not assigned.");
            return;
        }

        if (_sceneReferences.TryValidate(out string errorMessage) == false)
        {
            Debug.LogError($"CompositionRoot initialization failed.{Environment.NewLine}{errorMessage}", _sceneReferences);
            return;
        }

        _isInitializing = true;

        _saveService = new SaveService(ProgressStorageFactory.Create());
        _saveService.Load(OnProgressLoaded);
    }

    public void FlushSave()
    {
        _saveService?.Flush();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (_isInitialized)
        {
            _saveService?.Flush();
            UnsubscribeFromPresenterEvents();
            UnsubscribeFromStateEvents();

            _mainMenuPresenter?.Dispose();
            _victoryPresenter?.Dispose();
            _defeatPresenter?.Dispose();
            _runPresenter?.Dispose();
            _upgradeService?.Dispose();

            if (_adsProvider is IDisposable disposableAds)
            {
                disposableAds.Dispose();
            }
        }
        else
        {
            _saveService?.Flush();
        }
    }

    private void OnProgressLoaded(PlayerSaveData saveData)
    {
        if (_isDisposed || _isInitialized)
        {
            return;
        }

        _isInitializing = false;

        _playerProgress = new PlayerProgress(saveData);

        _settingsService = new SettingsService(
            saveData == null || saveData.SoundEnabled,
            saveData == null || saveData.MusicEnabled,
            saveData == null || saveData.VibrationEnabled);

        _wallet = _sceneReferences.Wallet;

        if (_wallet == null)
        {
            Debug.LogError("Wallet is not assigned in SceneReferences.");
            return;
        }

        _wallet.Initialize(saveData != null ? saveData.Coins : (int?)null);
        _upgradeService = new UpgradeService(
            _playerProgress,
            _wallet,
            _sceneReferences.PermanentUpgradeSettings,
            new UpgradeCostCalculator(_sceneReferences.PermanentUpgradeSettings.CostGrowth)
        );

        _playerHealth = _sceneReferences.PlayerHealth;

        _adsProvider = CreateAdsProvider();
        _leaderboardService = CreateLeaderboardProvider();
        _adsService = new AdsService(_adsProvider, _sceneReferences.AdsConfig);

        _secondChanceFlow = new SecondChanceFlow(
            _adsService,
            _sceneReferences.ObstacleFactory,
            _sceneReferences.Player,
            _playerHealth,
            _sceneReferences.AdsConfig.SecondChanceClearRadius);
        _multiplierFlow = new RewardedMultiplierFlow(
            _adsService, _wallet, _sceneReferences.RunRewardCollector,
            _sceneReferences.AdsConfig.VictoryCoinsMultiplier);
        _coinsFlow = new RewardedCoinsFlow(
            _adsService, _wallet, _sceneReferences.AdsConfig.MenuRewardCoins);

        CreateStates();
        CreateStateMachine();
        CreatePresenters();
        SubscribeToStateEvents();
        SubscribeToPresenterEvents();
        _isInitialized = true;

        if (saveData == null)
        {
            SaveCurrentProgress();
        }

        _stateMachine.Enter<BootstrapState>();
    }

    private void CreateStates()
    {
        _bootstrapState = new BootstrapState();
        _mainMenuState = new MainMenuState();
        _runState = new RunState(
            _playerProgress,
            _sceneReferences.Player,
            _playerHealth,
            _sceneReferences.LevelGenerator,
            _sceneReferences.TrackScroller,
            _upgradeService
        );
        _victoryState = new VictoryState();
        _defeatState = new DefeatState();
        _pausedState = new PausedState();
    }

    private void CreateStateMachine()
    {
        List<IState> states = new()
        {
            _bootstrapState,
            _mainMenuState,
            _runState,
            _victoryState,
            _defeatState,
            _pausedState
        };

        _stateMachine = new GameStateMachine(states);
    }

    private void CreatePresenters()
    {
        _mainMenuPresenter = new MainMenuPresenter(_sceneReferences.MainMenuScreen, _upgradeService, _coinsFlow);
        _runPresenter = new RunPresenter(_sceneReferences.RunScreen, _playerHealth, _playerProgress, _wallet, _sceneReferences.TrackScroller, _sceneReferences.LevelGenerator, _sceneReferences.BallQueue);
        _victoryPresenter = new VictoryPresenter(_sceneReferences.VictoryScreen, _sceneReferences.RunRewardCollector, _multiplierFlow);
        _defeatPresenter = new DefeatPresenter(_sceneReferences.DefeatScreen, _secondChanceFlow);
        _settingsPresenter = new SettingsPresenter(_sceneReferences.SettingsScreen, _settingsService);
    }

    private void SubscribeToStateEvents()
    {
        _bootstrapState.Completed += OnBootstrapCompleted;
        _playerHealth.Died += OnPlayerDied;
        _runState.Won += OnRunWon;
        _runState.Lost += OnRunLost;
        _sceneReferences.LevelGenerator.Finished += OnLevelFinished;
        _playerProgress.LevelChanged += OnProgressChanged;
        _playerProgress.UpgradeLevelChanged += OnUpgradeLevelChanged;
        _playerProgress.TutorialCompleted += OnTutorialCompleted;
        _playerProgress.BestLevelChanged += OnBestLevelChanged;

        _mainMenuState.Entered += _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited += _mainMenuPresenter.HandleStateExited;
        _runState.Entered += _runPresenter.HandleStateEntered;
        _runState.Entered += OnRunEntered;
        _secondChanceFlow.SecondChanceGranted += OnSecondChanceGranted;
        _runState.Exited += _runPresenter.HandleStateExited;
        _victoryState.Entered += _victoryPresenter.HandleStateEntered;
        _victoryState.Exited += _victoryPresenter.HandleStateExited;
        _defeatState.Entered += _defeatPresenter.HandleStateEntered;
        _defeatState.Exited += _defeatPresenter.HandleStateExited;

        _multiplierFlow.BonusGranted += OnVictoryBonusGranted;

        _runPresenter.SettingsRequested += OnRunSettingsRequested;
        _mainMenuPresenter.SettingsRequested += OnMenuSettingsRequested;
        _pausedState.Entered += OnPausedStateEntered;
        _pausedState.Exited += OnPausedStateExited;
        _settingsPresenter.CloseRequested += OnSettingsCloseRequested;
        _settingsPresenter.ExitToMenuRequested += OnSettingsExitToMenuRequested;
        _settingsService.SoundEnabledChanged += OnSettingsChanged;
        _settingsService.MusicEnabledChanged += OnSettingsChanged;
        _settingsService.VibrationEnabledChanged += OnSettingsChanged;
    }

    private void UnsubscribeFromStateEvents()
    {
        _bootstrapState.Completed -= OnBootstrapCompleted;
        _playerHealth.Died -= OnPlayerDied;
        _runState.Won -= OnRunWon;
        _runState.Lost -= OnRunLost;
        _sceneReferences.LevelGenerator.Finished -= OnLevelFinished;
        _playerProgress.LevelChanged -= OnProgressChanged;
        _playerProgress.UpgradeLevelChanged -= OnUpgradeLevelChanged;
        _playerProgress.TutorialCompleted -= OnTutorialCompleted;
        _playerProgress.BestLevelChanged -= OnBestLevelChanged;

        _mainMenuState.Entered -= _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited -= _mainMenuPresenter.HandleStateExited;
        _runState.Entered -= _runPresenter.HandleStateEntered;
        _runState.Entered -= OnRunEntered;
        _secondChanceFlow.SecondChanceGranted -= OnSecondChanceGranted;
        _runState.Exited -= _runPresenter.HandleStateExited;
        _victoryState.Entered -= _victoryPresenter.HandleStateEntered;
        _victoryState.Exited -= _victoryPresenter.HandleStateExited;
        _defeatState.Entered -= _defeatPresenter.HandleStateEntered;
        _defeatState.Exited -= _defeatPresenter.HandleStateExited;

        _multiplierFlow.BonusGranted -= OnVictoryBonusGranted;

        _runPresenter.SettingsRequested -= OnRunSettingsRequested;
        _mainMenuPresenter.SettingsRequested -= OnMenuSettingsRequested;
        _pausedState.Entered -= OnPausedStateEntered;
        _pausedState.Exited -= OnPausedStateExited;
        _settingsPresenter.CloseRequested -= OnSettingsCloseRequested;
        _settingsPresenter.ExitToMenuRequested -= OnSettingsExitToMenuRequested;
        _settingsService.SoundEnabledChanged -= OnSettingsChanged;
        _settingsService.MusicEnabledChanged -= OnSettingsChanged;
        _settingsService.VibrationEnabledChanged -= OnSettingsChanged;
    }

    private void SubscribeToPresenterEvents()
    {
        _mainMenuPresenter.StartRequested += OnStartRequested;
        _victoryPresenter.MainMenuRequested += OnVictoryMainMenuRequested;
        _defeatPresenter.MainMenuRequested += OnDefeatMainMenuRequested;
        _defeatPresenter.RestartRequested += OnDefeatRestartRequested;
    }

    private void UnsubscribeFromPresenterEvents()
    {
        _mainMenuPresenter.StartRequested -= OnStartRequested;
        _victoryPresenter.MainMenuRequested -= OnVictoryMainMenuRequested;
        _defeatPresenter.MainMenuRequested -= OnDefeatMainMenuRequested;
        _defeatPresenter.RestartRequested -= OnDefeatRestartRequested;
    }

    private void SaveCurrentProgress()
    {
        if (_saveService == null || _playerProgress == null || _wallet == null || _settingsService == null)
        {
            return;
        }

        PlayerSaveData data = _playerProgress.CreateSaveData(_wallet.Balance);
        data.SoundEnabled = _settingsService.SoundEnabled;
        data.MusicEnabled = _settingsService.MusicEnabled;
        data.VibrationEnabled = _settingsService.VibrationEnabled;
        _saveService.Save(data);
    }

    private void OnProgressChanged(int level)
    {
        SaveCurrentProgress();
        _saveService?.Flush();
    }

    private void OnUpgradeLevelChanged(PermanentUpgradeType type, int level)
    {
        SaveCurrentProgress();
        _saveService?.Flush();
    }

    private void OnTutorialCompleted()
    {
        SaveCurrentProgress();
        _saveService?.Flush();
    }

    private void OnPlayerDied() => _runState.RequestLose();
    private void OnLevelFinished() => _runState.RequestWin();
    private void OnBootstrapCompleted() => _stateMachine.Enter<MainMenuState>();
    private void OnStartRequested() => _stateMachine.Enter<RunState>();

    private void OnRunWon()
    {
        _playerProgress.CompleteCurrentLevel();
        _stateMachine.Enter<VictoryState>();
    }

    private void OnRunLost()
    {
        _stateMachine.Enter<DefeatState>();
        SaveCurrentProgress();
        _saveService?.Flush();
    }

    private void OnRunEntered()
    {
        _secondChanceFlow.SetContext(_runState.Context);
    }

    private void OnSecondChanceGranted()
    {
        _stateMachine.Enter<RunState, RunEnterMode>(RunEnterMode.Resume);
    }

    private void OnVictoryBonusGranted(int bonus)
    {
        ReturnToMainMenu();
    }

    private void OnVictoryMainMenuRequested() => ReturnToMainMenu();
    private void OnDefeatMainMenuRequested() => ReturnToMainMenu();

    private void OnDefeatRestartRequested()
    {
        _runState.ClearRun();
        _stateMachine.Enter<RunState>();
    }

    private void OnRunSettingsRequested() => _stateMachine.Enter<PausedState>();

    private void OnMenuSettingsRequested()
    {
        _settingsPauseContext = false;
        _settingsPresenter.Show(false);
    }

    private void OnPausedStateEntered()
    {
        _settingsPauseContext = true;
        _settingsPresenter.Show(true);
    }

    private void OnPausedStateExited()
    {
        _settingsPauseContext = false;
        _settingsPresenter.Hide();
    }

    private void OnSettingsCloseRequested()
    {
        if (_settingsPauseContext)
        {
            _stateMachine.Enter<RunState, RunEnterMode>(RunEnterMode.Resume);
        }
        else
        {
            _settingsPresenter.Hide();
        }
    }

    private void OnSettingsExitToMenuRequested()
    {
        _wallet.Remove(_sceneReferences.RunRewardCollector.CollectedThisRun);
        SaveCurrentProgress();
        _saveService?.Flush();
        ReturnToMainMenu();
    }

    private void OnSettingsChanged(bool enabled)
    {
        SaveCurrentProgress();
        _saveService?.Flush();
    }

    private void ReturnToMainMenu()
    {
        _adsService.TryShowInterstitial();

        _runState.ClearRun();
        _stateMachine.Enter<MainMenuState>();
    }

    private IAdsService CreateAdsProvider()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
    return new YandexAdsService();
#else
        return new EditorAdsService();
#endif
    }

    private void OnBestLevelChanged(int bestLevel)
    {
        _leaderboardService.SubmitScore(bestLevel);
    }

    private ILeaderboardService CreateLeaderboardProvider()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexLeaderboardService();
#else
        return new EditorLeaderboardService();
#endif
    }
}