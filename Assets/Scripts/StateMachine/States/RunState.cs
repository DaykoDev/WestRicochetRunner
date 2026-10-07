using System;

public class RunState : IState
{
    private readonly PlayerProgress _playerProgress;

    private RunContext _context;

    public event Action Entered;
    public event Action Exited;
    public event Action Won;
    public event Action Lost;

    public RunState(PlayerProgress playerProgress)
    {
        _playerProgress = playerProgress;
    }

    public void Enter()
    {
        CreateRunContext();

        // TODO: Здесь будет подключён LevelGenerator.
        // _levelGenerator.BuildLevel(_context.Level, _context.Seed);

        Entered?.Invoke();
    }

    public void Exit()
    {
        StopRunSystems();
        DestroyRunContext();

        Exited?.Invoke();
    }

    public void RequestWin()
    {
        Won?.Invoke();
    }

    public void RequestLose()
    {
        Lost?.Invoke();
    }

    private void CreateRunContext()
    {
        RunModifiers modifiers = new RunModifiers();

        _context = new RunContext(_playerProgress.CurrentLevel, _playerProgress.CurrentLevelSeed, modifiers);
    }

    private void StopRunSystems()
    {
        // TODO: Остановка gameplay-систем.
        // Player, BallLauncher, LevelGenerator и т.д.
    }

    private void DestroyRunContext()
    {
        _context = null;
    }
}