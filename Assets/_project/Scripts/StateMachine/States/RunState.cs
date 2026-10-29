using System;
using UnityEngine;

public class RunState : IState, IPayloadedState<RunEnterMode>
{
    private readonly PlayerProgress _playerProgress;
    private readonly Player _player;
    private readonly PlayerHealth _playerHealth;
    private readonly LevelGenerator _levelGenerator;
    private readonly TrackScroller _trackScroller;
    private readonly UpgradeService _upgradeService;
    private readonly RunRewardCollector _rewardCollector;

    private RunContext _context;
    private bool _isRunning;

    public RunState(PlayerProgress playerProgress, Player player, PlayerHealth playerHealth, LevelGenerator levelGenerator, TrackScroller trackScroller, UpgradeService upgradeService, RunRewardCollector rewardCollector)
    {
        _playerProgress = playerProgress;
        _player = player;
        _playerHealth = playerHealth;
        _levelGenerator = levelGenerator;
        _trackScroller = trackScroller;
        _upgradeService = upgradeService;
        _rewardCollector = rewardCollector;
    }

    public RunContext Context => _context;

    public event Action Entered;
    public event Action Exited;
    public event Action Won;
    public event Action Lost;

    public event Action Failed;

    public void Enter()
    {
        Enter(RunEnterMode.NewRun);
    }

    public void Enter(RunEnterMode mode)
    {
        if (mode == RunEnterMode.Resume)
        {
            ResumeRun();
            return;
        }

        StartNewRun();
    }

    private void StartNewRun()
    {
        Time.timeScale = 1f;
        _trackScroller.ResetPosition();
        _isRunning = true;

        CreateRunContext();

        _playerHealth.ResetHealth();
        _player.ResetToStart();
        _player.SetRunModifiers(_context.Modifiers);

        bool isLevelBuilt = _levelGenerator.BuildLevel(_context.Level, _context.Seed, _context.Modifiers);

        if (isLevelBuilt == false)
        {
            _isRunning = false;
            Failed?.Invoke();
            return;
        }

        StartRunSystems();
        Entered?.Invoke();
    }

    private void ResumeRun()
    {
        Time.timeScale = 1f;
        _isRunning = true;

        StartRunSystems();
        Entered?.Invoke();
    }

    public void Exit()
    {
        _isRunning = false;
        _trackScroller.StopScrolling();
        _player.SetControlEnabled(false);

        Exited?.Invoke();
    }

    public void RequestWin()
    {
        if (_isRunning == false)
        {
            return;
        }

        Won?.Invoke();
    }

    public void RequestLose()
    {
        if (_isRunning == false)
        {
            return;
        }

        Lost?.Invoke();
    }

    public void ClearRun()
    {
        _isRunning = false;
        _trackScroller.StopScrolling();
        _player.ClearRun();
        _rewardCollector.ClearPickups();
        _levelGenerator.ClearLevel();
        _trackScroller.ResetPosition();
        _context = null;
    }

    private void StartRunSystems()
    {
        _trackScroller.StartScrolling(_player.ForwardSpeed);
        _player.SetControlEnabled(true);
    }

    private void CreateRunContext()
    {
        RunModifiers modifiers = new RunModifiers();
        _upgradeService.ApplyPermanentUpgrades(modifiers);

        _context = new RunContext(_playerProgress.CurrentLevel, _playerProgress.CurrentLevelSeed, modifiers);
    }
}