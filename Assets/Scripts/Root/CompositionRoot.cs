using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositionRoot : IDisposable
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

    private GameFlow _gameFlow;
    private SettingsFlow _settingsFlow;
    private ProgressSaver _progressSaver;

    private bool _isInitializing;
    private bool _isInitialized;
    private bool _isDisposed;

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

        _saveService = new SaveService(CreateProgressStorage());
        _saveService.Load(OnProgressLoaded);
    }

    public void FlushSave()
    {
        if (_isInitialized)
        {
            _progressSaver.SaveAndFlush();
        }
        else
        {
            _saveService?.Flush();
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (_isInitialized == false)
        {
            _saveService?.Flush();
            return;
        }

        _progressSaver.SaveAndFlush();

        UnsubscribeFromPresenterEvents();
        UnsubscribeFromStateEvents();

        _settingsFlow.Dispose();
        _gameFlow.Dispose();
        _progressSaver.Dispose();

        _mainMenuPresenter.Dispose();
        _victoryPresenter.Dispose();
        _defeatPresenter.Dispose();
        _runPresenter.Dispose();
        _settingsPresenter.Dispose();

        _upgradeService.Dispose();

        if (_adsProvider is IDisposable disposableAds)
        {
            disposableAds.Dispose();
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
            _sceneReferences.AudioMixer);

        _wallet = _sceneReferences.Wallet;

        if (_wallet == null)
        {
            Debug.LogError("Wallet is not assigned in SceneReferences.");
            return;
        }

        int? savedBalance = saveData != null ? saveData.Coins : (int?)null;

        _wallet.Initialize(savedBalance);

        _upgradeService = new UpgradeService(
            _playerProgress,
            _wallet,
            _sceneReferences.PermanentUpgradeSettings,
            new UpgradeCostCalculator(_sceneReferences.PermanentUpgradeSettings.CostGrowth));

        _playerHealth = _sceneReferences.PlayerHealth;

        _adsProvider = CreateAdsProvider();
        _leaderboardService = CreateLeaderboardProvider();

        _adsService = new AdsService(_adsProvider, _sceneReferences.AdsConfig);

        CreateRewardFlows();
        CreateStates();
        CreateStateMachine();
        CreatePresenters();
        CreateFlows();

        SubscribeToStateEvents();
        SubscribeToPresenterEvents();

        _isInitialized = true;

        _stateMachine.Enter<BootstrapState>();
    }

    private void CreateRewardFlows()
    {
        _secondChanceFlow = new SecondChanceFlow(
            _adsService,
            _sceneReferences.ObstacleFactory,
            _sceneReferences.Player,
            _playerHealth,
            _sceneReferences.AdsConfig.SecondChanceClearRadius);

        _multiplierFlow = new RewardedMultiplierFlow(
            _adsService,
            _wallet,
            _sceneReferences.RunRewardCollector,
            _sceneReferences.AdsConfig.VictoryCoinsMultiplier);

        _coinsFlow = new RewardedCoinsFlow(
            _adsService,
            _wallet,
            _sceneReferences.AdsConfig.MenuRewardCoins);
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
        _mainMenuPresenter = new MainMenuPresenter(
            _sceneReferences.MainMenuScreen,
            _upgradeService,
            _coinsFlow);

        _runPresenter = new RunPresenter(
            _sceneReferences.RunScreen,
            _playerHealth,
            _playerProgress,
            _wallet,
            _sceneReferences.TrackScroller,
            _sceneReferences.LevelGenerator,
            _sceneReferences.BallQueue);

        _victoryPresenter = new VictoryPresenter(
            _sceneReferences.VictoryScreen,
            _sceneReferences.RunRewardCollector,
            _multiplierFlow);

        _defeatPresenter = new DefeatPresenter(
            _sceneReferences.DefeatScreen,
            _secondChanceFlow);

        _settingsPresenter = new SettingsPresenter(
            _sceneReferences.SettingsScreen,
            _settingsService);
    }

    private void CreateFlows()
    {
        _gameFlow = new GameFlow(
            _stateMachine,
            _playerProgress,
            _playerHealth,
            _sceneReferences.LevelGenerator,
            _runState,
            _secondChanceFlow,
            _adsService,
            _wallet,
            _sceneReferences.RunRewardCollector);

        _settingsFlow = new SettingsFlow(
            _settingsPresenter,
            _pausedState,
            _gameFlow);

        _progressSaver = new ProgressSaver(
            _saveService,
            _playerProgress,
            _wallet,
            _settingsService,
            _mainMenuState,
            _runState,
            _coinsFlow,
            _multiplierFlow);
    }

    private void SubscribeToStateEvents()
    {
        _bootstrapState.Completed += OnBootstrapCompleted;

        _mainMenuState.Entered += _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited += _mainMenuPresenter.HandleStateExited;

        _runState.Entered += _runPresenter.HandleStateEntered;
        _runState.Entered += OnRunEntered;
        _runState.Exited += _runPresenter.HandleStateExited;

        _victoryState.Entered += _victoryPresenter.HandleStateEntered;
        _victoryState.Exited += _victoryPresenter.HandleStateExited;

        _defeatState.Entered += _defeatPresenter.HandleStateEntered;
        _defeatState.Exited += _defeatPresenter.HandleStateExited;

        _playerProgress.BestLevelChanged += _leaderboardService.SubmitScore;
    }

    private void UnsubscribeFromStateEvents()
    {
        _bootstrapState.Completed -= OnBootstrapCompleted;

        _mainMenuState.Entered -= _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited -= _mainMenuPresenter.HandleStateExited;

        _runState.Entered -= _runPresenter.HandleStateEntered;
        _runState.Entered -= OnRunEntered;
        _runState.Exited -= _runPresenter.HandleStateExited;

        _victoryState.Entered -= _victoryPresenter.HandleStateEntered;
        _victoryState.Exited -= _victoryPresenter.HandleStateExited;

        _defeatState.Entered -= _defeatPresenter.HandleStateEntered;
        _defeatState.Exited -= _defeatPresenter.HandleStateExited;

        _playerProgress.BestLevelChanged -= _leaderboardService.SubmitScore;
    }

    private void SubscribeToPresenterEvents()
    {
        _mainMenuPresenter.StartRequested += _gameFlow.StartRun;
        _victoryPresenter.MainMenuRequested += _gameFlow.ReturnToMainMenu;
        _defeatPresenter.MainMenuRequested += _gameFlow.ReturnToMainMenu;
        _defeatPresenter.RestartRequested += _gameFlow.RestartRun;
        _multiplierFlow.BonusGranted += _gameFlow.CompleteVictoryBonus;
        _runPresenter.SettingsRequested += _settingsFlow.OpenFromRun;
        _mainMenuPresenter.SettingsRequested += _settingsFlow.OpenFromMenu;
    }

    private void UnsubscribeFromPresenterEvents()
    {
        _mainMenuPresenter.StartRequested -= _gameFlow.StartRun;
        _victoryPresenter.MainMenuRequested -= _gameFlow.ReturnToMainMenu;
        _defeatPresenter.MainMenuRequested -= _gameFlow.ReturnToMainMenu;
        _defeatPresenter.RestartRequested -= _gameFlow.RestartRun;
        _multiplierFlow.BonusGranted -= _gameFlow.CompleteVictoryBonus;
        _runPresenter.SettingsRequested -= _settingsFlow.OpenFromRun;
        _mainMenuPresenter.SettingsRequested -= _settingsFlow.OpenFromMenu;
    }

    private IProgressStorage CreateProgressStorage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexCloudProgressStorage();
#else
        return new LocalProgressStorage();
#endif
    }

    private IAdsService CreateAdsProvider()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexAdsService();
#else
        return new EditorAdsService();
#endif
    }

    private ILeaderboardService CreateLeaderboardProvider()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexLeaderboardService();
#else
        return new EditorLeaderboardService();
#endif
    }

    private void OnBootstrapCompleted()
    {
        _gameFlow.EnterMainMenuFromBootstrap();
    }

    private void OnRunEntered()
    {
        _secondChanceFlow.SetContext(_runState.Context);
    }
}