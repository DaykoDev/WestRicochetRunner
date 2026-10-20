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
    private TrackScroller _trackScroller;

    private BootstrapState _bootstrapState;
    private MainMenuState _mainMenuState;
    private RunState _runState;
    private VictoryState _victoryState;
    private DefeatState _defeatState;

    private MainMenuPresenter _mainMenuPresenter;
    private RunPresenter _runPresenter;
    private VictoryPresenter _victoryPresenter;
    private DefeatPresenter _defeatPresenter;

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
            new UpgradeCostCalculator()
        );

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
        _playerHealth = _sceneReferences.Player.GetComponent<PlayerHealth>();
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
    }

    private void CreateStateMachine()
    {
        List<IState> states = new()
        {
            _bootstrapState,
            _mainMenuState,
            _runState,
            _victoryState,
            _defeatState
        };

        _stateMachine = new GameStateMachine(states);
    }

    private void CreatePresenters()
    {
        _mainMenuPresenter = new MainMenuPresenter(_sceneReferences.MainMenuScreen, _upgradeService);
        _runPresenter = new RunPresenter(_sceneReferences.RunScreen, _playerHealth, _playerProgress, _wallet);
        _victoryPresenter = new VictoryPresenter(_sceneReferences.VictoryScreen);
        _defeatPresenter = new DefeatPresenter(_sceneReferences.DefeatScreen);
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
        _wallet.BalanceChanged += OnWalletBalanceChanged;

        _mainMenuState.Entered += _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited += _mainMenuPresenter.HandleStateExited;
        _runState.Entered += _runPresenter.HandleStateEntered;
        _runState.Exited += _runPresenter.HandleStateExited;
        _victoryState.Entered += _victoryPresenter.HandleStateEntered;
        _victoryState.Exited += _victoryPresenter.HandleStateExited;
        _defeatState.Entered += _defeatPresenter.HandleStateEntered;
        _defeatState.Exited += _defeatPresenter.HandleStateExited;
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
        _wallet.BalanceChanged -= OnWalletBalanceChanged;

        _mainMenuState.Entered -= _mainMenuPresenter.HandleStateEntered;
        _mainMenuState.Exited -= _mainMenuPresenter.HandleStateExited;
        _runState.Entered -= _runPresenter.HandleStateEntered;
        _runState.Exited -= _runPresenter.HandleStateExited;
        _victoryState.Entered -= _victoryPresenter.HandleStateEntered;
        _victoryState.Exited -= _victoryPresenter.HandleStateExited;
        _defeatState.Entered -= _defeatPresenter.HandleStateEntered;
        _defeatState.Exited -= _defeatPresenter.HandleStateExited;
    }

    private void SubscribeToPresenterEvents()
    {
        _mainMenuPresenter.StartRequested += OnStartRequested;
        _victoryPresenter.MainMenuRequested += OnVictoryMainMenuRequested;
        _defeatPresenter.MainMenuRequested += OnDefeatMainMenuRequested;
    }

    private void UnsubscribeFromPresenterEvents()
    {
        _mainMenuPresenter.StartRequested -= OnStartRequested;
        _victoryPresenter.MainMenuRequested -= OnVictoryMainMenuRequested;
        _defeatPresenter.MainMenuRequested -= OnDefeatMainMenuRequested;
    }

    private void SaveCurrentProgress()
    {
        if (_saveService == null || _playerProgress == null || _wallet == null)
        {
            return;
        }

        PlayerSaveData data = _playerProgress.CreateSaveData(_wallet.Balance);
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

    private void OnWalletBalanceChanged(int balance)
    {
        SaveCurrentProgress();
    }

    private void OnPlayerDied() => _runState.RequestLose();
    private void OnLevelFinished() => _runState.RequestWin();
    private void OnBootstrapCompleted() => _stateMachine.Enter<MainMenuState>();
    private void OnStartRequested() => _stateMachine.Enter<RunState>();

    private void OnRunWon()
    {
        Time.timeScale = 0f;
        _playerProgress.CompleteCurrentLevel();
        _stateMachine.Enter<VictoryState>();
    }

    private void OnRunLost()
    {
        Time.timeScale = 0f;
        _stateMachine.Enter<DefeatState>();
    }

    private void OnVictoryMainMenuRequested() => ReturnToMainMenu();
    private void OnDefeatMainMenuRequested() => ReturnToMainMenu();

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        _runState.ClearRun();
        _stateMachine.Enter<MainMenuState>();
    }
}