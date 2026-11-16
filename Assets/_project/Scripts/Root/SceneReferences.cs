using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using WestRicochet.Gameplay.Balls.Shooting;
using WestRicochet.Gameplay.Level.Generation;
using WestRicochet.Gameplay.Level.Runtime;
using WestRicochet.Gameplay.Obstacles.Spawning;
using WestRicochet.Gameplay.Player;
using WestRicochet.Gameplay.Player.Damage;
using WestRicochet.Gameplay.Rewards;
using WestRicochet.Gameplay.Rewards.Flow;
using WestRicochet.Infrastructure.Ads;
using WestRicochet.Meta;
using WestRicochet.UI.Screens;
using YG;

namespace WestRicochet.Root
{
    public class SceneReferences : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private MainMenuScreen _mainMenuScreen;
        [SerializeField] private RunScreen _runScreen;
        [SerializeField] private VictoryScreen _victoryScreen;
        [SerializeField] private DefeatScreen _defeatScreen;
        [SerializeField] private SettingsScreen _settingsScreen;
        [SerializeField] private LeaderboardScreen _leaderboardScreen;

        [Header("Player")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private PlayerHealth _playerHealth;

        [Header("Gameplay")]
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private TrackScroller _trackScroller;
        [SerializeField] private BallQueue _ballQueue;
        [SerializeField] private Wallet _wallet;
        [SerializeField] private PermanentUpgradeSettings _permanentUpgradeSettings;
        [SerializeField] private LevelEconomySettings _levelEconomySettings;
        [SerializeField] private RunRewardCollector _runRewardCollector;
        [SerializeField] private ObstacleAreaReleaser _obstacleAreaReleaser;

        [Header("Services")]
        [SerializeField] private AdsConfig _adsConfig;
        [SerializeField] private LeaderboardYG _leaderboard;

        [SerializeField] private AudioMixer _audioMixer;

        public MainMenuScreen MainMenuScreen => _mainMenuScreen;
        public RunScreen RunScreen => _runScreen;
        public VictoryScreen VictoryScreen => _victoryScreen;
        public DefeatScreen DefeatScreen => _defeatScreen;
        public SettingsScreen SettingsScreen => _settingsScreen;
        public LeaderboardScreen LeaderboardScreen => _leaderboardScreen;

        public PlayerController Player => _player;
        public PlayerHealth PlayerHealth => _playerHealth;

        public LevelGenerator LevelGenerator => _levelGenerator;
        public TrackScroller TrackScroller => _trackScroller;
        public BallQueue BallQueue => _ballQueue;
        public Wallet Wallet => _wallet;
        public PermanentUpgradeSettings PermanentUpgradeSettings => _permanentUpgradeSettings;
        public LevelEconomySettings LevelEconomySettings => _levelEconomySettings;
        public RunRewardCollector RunRewardCollector => _runRewardCollector;
        public ObstacleAreaReleaser ObstacleAreaReleaser => _obstacleAreaReleaser;

        public AdsConfig AdsConfig => _adsConfig;
        public LeaderboardYG LeaderboardYG => _leaderboard;

        public AudioMixer AudioMixer => _audioMixer;

        public bool TryValidate(out string errorMessage)
        {
            List<string> errors = new();

            ValidateReference(_mainMenuScreen, nameof(_mainMenuScreen), errors);
            ValidateReference(_runScreen, nameof(_runScreen), errors);
            ValidateReference(_victoryScreen, nameof(_victoryScreen), errors);
            ValidateReference(_defeatScreen, nameof(_defeatScreen), errors);
            ValidateReference(_settingsScreen, nameof(_settingsScreen), errors);
            ValidateReference(_leaderboardScreen, nameof(_leaderboardScreen), errors);

            ValidateReference(_player, nameof(_player), errors);
            ValidateReference(_playerHealth, nameof(_playerHealth), errors);

            ValidateReference(_levelGenerator, nameof(_levelGenerator), errors);
            ValidateReference(_trackScroller, nameof(_trackScroller), errors);
            ValidateReference(_ballQueue, nameof(_ballQueue), errors);
            ValidateReference(_wallet, nameof(_wallet), errors);
            ValidateReference(_permanentUpgradeSettings, nameof(_permanentUpgradeSettings), errors);
            ValidateReference(_levelEconomySettings, nameof(_levelEconomySettings), errors);
            ValidateReference(_runRewardCollector, nameof(_runRewardCollector), errors);
            ValidateReference(_obstacleAreaReleaser, nameof(_obstacleAreaReleaser), errors);

            ValidateReference(_adsConfig, nameof(_adsConfig), errors);
            ValidateReference(_leaderboard, nameof(_leaderboard), errors);

            ValidateReference(_audioMixer, nameof(_audioMixer), errors);

            if (errors.Count == 0)
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = "SceneReferences has invalid configuration:" +
                Environment.NewLine +
                "- " +
                string.Join(Environment.NewLine + "- ", errors);

            return false;
        }

        private static void ValidateReference(UnityEngine.Object reference, string referenceName, List<string> errors)
        {
            if (reference == null)
            {
                errors.Add($"{referenceName} is not assigned.");
            }
        }
    }
}