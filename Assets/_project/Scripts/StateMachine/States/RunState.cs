using System;
using UnityEngine;
using WestRicochet.Gameplay.Level.Generation;
using WestRicochet.Gameplay.Level.Runtime;
using WestRicochet.Gameplay.Player;
using WestRicochet.Gameplay.Player.Damage;
using WestRicochet.Gameplay.Rewards.Flow;
using WestRicochet.Gameplay.Run;
using WestRicochet.Gameplay.Upgrades;
using WestRicochet.Meta;

namespace WestRicochet.StateMachine.States
{
    public class RunState : IState, IPayloadedState<RunEnterMode>
    {
        private readonly PlayerProgress _playerProgress;
        private readonly PlayerController _player;
        private readonly PlayerHealth _playerHealth;
        private readonly LevelGenerator _levelGenerator;
        private readonly TrackScroller _trackScroller;
        private readonly UpgradeService _upgradeService;
        private readonly RunRewardCollector _rewardCollector;

        private RunContext _context;
        private bool _isRunning;

        public RunState(
            PlayerProgress playerProgress,
            PlayerController player,
            PlayerHealth playerHealth,
            LevelGenerator levelGenerator,
            TrackScroller trackScroller,
            UpgradeService upgradeService,
            RunRewardCollector rewardCollector)
        {
            _playerProgress = playerProgress;
            _player = player;
            _playerHealth = playerHealth;
            _levelGenerator = levelGenerator;
            _trackScroller = trackScroller;
            _upgradeService = upgradeService;
            _rewardCollector = rewardCollector;
        }

        public event Action Entered;
        public event Action Exited;
        public event Action Won;
        public event Action Lost;

        public event Action Failed;

        public RunContext Context => _context;
        public bool HasPreparedRun { get; private set; }

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
        public bool PrepareRun()
        {
            if (HasPreparedRun)
            {
                return true;
            }

            Time.timeScale = 1f;
            _isRunning = false;

            _trackScroller.ResetPosition();
            _player.ClearRun();

            CreateRunContext();

            _playerHealth.ResetHealth();
            _player.ResetToStart();
            _player.ConfigureForLevel(_context.Level);
            _player.SetRunModifiers(_context.Modifiers);

            _rewardCollector.BeginRun();

            bool isLevelBuilt = _levelGenerator.BuildLevel(_context.Level, _context.Seed, _context.Modifiers);

            if (isLevelBuilt == false)
            {
                ClearRun();
                return false;
            }

            HasPreparedRun = true;
            return true;
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
            HasPreparedRun = false;

            _trackScroller.StopScrolling();
            _player.ClearRun();
            _rewardCollector.ClearPickups();
            _levelGenerator.ClearLevel();
            _trackScroller.ResetPosition();
            _context = null;
        }

        private void StartNewRun()
        {
            Time.timeScale = 1f;

            if (HasPreparedRun == false && PrepareRun() == false)
            {
                _isRunning = false;
                Failed?.Invoke();
                return;
            }

            HasPreparedRun = false;
            _isRunning = true;

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
}