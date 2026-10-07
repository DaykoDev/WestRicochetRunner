using System.Collections.Generic;

public class CompositionRoot
{
    private readonly SceneReferences _sceneReferences;

    private GameStateMachine _stateMachine;
    private PlayerProgress _playerProgress;

    private BootstrapState _bootstrapState;
    private MainMenuState _mainMenuState;
    private RunState _runState;
    private VictoryState _victoryState;
    private DefeatState _defeatState;

    private MainMenuPresenter _mainMenuPresenter;
    private RunPresenter _runPresenter;
    private VictoryPresenter _victoryPresenter;
    private DefeatPresenter _defeatPresenter;

    public CompositionRoot(SceneReferences sceneReferences)
    {
        _sceneReferences = sceneReferences;
    }

    public void Initialize()
    {
        CreateProgress();
        CreateStates();
        CreateStateMachine();
        CreatePresenters();

        SubscribeToStateEvents();
        SubscribeToPresenterEvents();

        _stateMachine.Enter<BootstrapState>();
    }

    public void Dispose()
    {
        UnsubscribeFromPresenterEvents();
        UnsubscribeFromStateEvents();

        _mainMenuPresenter.Dispose();
        _victoryPresenter.Dispose();
        _defeatPresenter.Dispose();
    }

    private void CreateProgress()
    {
        _playerProgress = new PlayerProgress();
    }

    private void CreateStates()
    {
        _bootstrapState = new BootstrapState();
        _mainMenuState = new MainMenuState();
        _runState = new RunState(_playerProgress);
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
        _mainMenuPresenter = new MainMenuPresenter(_sceneReferences.MainMenuScreen);

        _runPresenter = new RunPresenter(_sceneReferences.RunScreen);

        _victoryPresenter = new VictoryPresenter(_sceneReferences.VictoryScreen);

        _defeatPresenter = new DefeatPresenter(_sceneReferences.DefeatScreen);
    }

    private void SubscribeToStateEvents()
    {
        _bootstrapState.Completed += OnBootstrapCompleted;

        _runState.Won += OnRunWon;
        _runState.Lost += OnRunLost;

        _victoryState.MainMenuRequested += OnVictoryMainMenuRequested;
        _defeatState.MainMenuRequested += OnDefeatMainMenuRequested;

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

        _runState.Won -= OnRunWon;
        _runState.Lost -= OnRunLost;

        _victoryState.MainMenuRequested -= OnVictoryMainMenuRequested;
        _defeatState.MainMenuRequested -= OnDefeatMainMenuRequested;

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

    private void OnBootstrapCompleted()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    private void OnStartRequested()
    {
        _stateMachine.Enter<RunState>();
    }

    private void OnRunWon()
    {
        _playerProgress.CompleteCurrentLevel();
        _stateMachine.Enter<VictoryState>();
    }

    private void OnRunLost()
    {
        _stateMachine.Enter<DefeatState>();
    }

    private void OnVictoryMainMenuRequested()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    private void OnDefeatMainMenuRequested()
    {
        _stateMachine.Enter<MainMenuState>();
    }
}