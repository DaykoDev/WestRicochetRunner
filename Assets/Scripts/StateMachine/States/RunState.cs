using System;

public class RunState : IState
{
    private readonly PlayerProgress _playerProgress;
    private readonly Player _player;

    private RunContext _context;

    public RunState(PlayerProgress playerProgress, Player player)
    {
        _playerProgress = playerProgress;
        _player = player;
    }

    public event Action Entered;
    public event Action Exited;
    public event Action Won;
    public event Action Lost;

    public void Enter()
    {
        CreateRunContext();

        _player.ResetToStart();
        StartRunSystems();

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