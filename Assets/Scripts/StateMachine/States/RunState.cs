using System;

public class RunState : IState
{
    private readonly PlayerProgress _playerProgress;
    private readonly Player _player;
    private readonly PlayerHealth _playerHealth;
    private readonly LevelGenerator _levelGenerator;

    private RunContext _context;
    private bool _isRunning;

    public RunState(PlayerProgress playerProgress, Player player, PlayerHealth playerHealth, LevelGenerator levelGenerator)
    {
        _playerProgress = playerProgress;
        _player = player;
        _playerHealth = playerHealth;
        _levelGenerator = levelGenerator;
    }


    public event Action Entered;
    public event Action Exited;
    public event Action Won;
    public event Action Lost;

    public void Enter()
    {
        _isRunning = true;
        CreateRunContext();

        _playerHealth.ResetHealth();
        _player.ResetToStart();
        _player.SetRunModifiers(_context.Modifiers);
        _levelGenerator.BuildLevel(_context.Level, _context.Seed, _context.Modifiers);

        StartRunSystems();
        Entered?.Invoke();
    }

    public void Exit()
    {
        _isRunning = false;
        StopRunSystems();
        _levelGenerator.ClearLevel();
        DestroyRunContext();

        Exited?.Invoke();
    }

    public void RequestWin()
    {
        if (_isRunning == false)
            return;

        Won?.Invoke();
    }

    public void RequestLose()
    {
        if (_isRunning == false)
            return;

        Lost?.Invoke();
    }

    private void StartRunSystems()
    {
        _player.SetControlEnabled(true);
    }

    private void StopRunSystems()
    {
        _player.SetControlEnabled(false);
    }

    private void CreateRunContext()
    {
        RunModifiers modifiers = new RunModifiers();

        _context = new RunContext(_playerProgress.CurrentLevel, _playerProgress.CurrentLevelSeed, modifiers);
    }

    private void DestroyRunContext()
    {
        _context = null;
    }
}