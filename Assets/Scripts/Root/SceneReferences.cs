using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    [SerializeField] private MainMenuScreen _mainMenuScreen;
    [SerializeField] private RunScreen _runScreen;
    [SerializeField] private VictoryScreen _victoryScreen;
    [SerializeField] private DefeatScreen _defeatScreen;
    [SerializeField] private SettingsScreen _settingsScreen;

    [SerializeField] private Player _player;
    [SerializeField] private PlayerHealth _playerHealth;

    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private TrackScroller _trackScroller;
    [SerializeField] private BallQueue _ballQueue;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private PermanentUpgradeSettings _permanentUpgradeSettings;
    [SerializeField] private RunRewardCollector _runRewardCollector;
    [SerializeField] private ObstacleFactory _obstacleFactory;

    [SerializeField] private AdsConfig _adsConfig;

    public MainMenuScreen MainMenuScreen => _mainMenuScreen;
    public RunScreen RunScreen => _runScreen;
    public VictoryScreen VictoryScreen => _victoryScreen;
    public DefeatScreen DefeatScreen => _defeatScreen;
    public SettingsScreen SettingsScreen => _settingsScreen;

    public Player Player => _player;
    public PlayerHealth PlayerHealth => _playerHealth;

    public LevelGenerator LevelGenerator => _levelGenerator;
    public TrackScroller TrackScroller => _trackScroller;
    public BallQueue BallQueue => _ballQueue;
    public Wallet Wallet => _wallet;
    public PermanentUpgradeSettings PermanentUpgradeSettings => _permanentUpgradeSettings;
    public RunRewardCollector RunRewardCollector => _runRewardCollector;
    public ObstacleFactory ObstacleFactory => _obstacleFactory;

    public AdsConfig AdsConfig => _adsConfig;

    public bool TryValidate(out string errorMessage)
    {
        List<string> errors = new List<string>();

        if (_mainMenuScreen == null)
        {
            errors.Add($"{nameof(_mainMenuScreen)} is not assigned.");
        }

        if (_runScreen == null)
        {
            errors.Add($"{nameof(_runScreen)} is not assigned.");
        }

        if (_victoryScreen == null)
        {
            errors.Add($"{nameof(_victoryScreen)} is not assigned.");
        }

        if (_defeatScreen == null)
        {
            errors.Add($"{nameof(_defeatScreen)} is not assigned.");
        }

        if (_settingsScreen == null)
        {
            errors.Add($"{nameof(_settingsScreen)} is not assigned.");
        }

        if (_player == null)
        {
            errors.Add($"{nameof(_player)} is not assigned.");
        }

        if (_playerHealth == null)
        {
            errors.Add($"{nameof(_player)} is not assigned.");
        }

        if (_levelGenerator == null)
        {
            errors.Add($"{nameof(_levelGenerator)} is not assigned.");
        }

        if (_trackScroller == null)
        {
            errors.Add($"{nameof(_trackScroller)} is not assigned.");
        }

        if (_ballQueue == null)
        {
            errors.Add($"{nameof(_ballQueue)} is not assigned.");
        }

        if (_wallet == null)
        {
            errors.Add($"{nameof(_wallet)} is not assigned.");
        }

        if (_permanentUpgradeSettings == null)
        {
            errors.Add($"{nameof(_permanentUpgradeSettings)} is not assigned.");
        }

        if (_runRewardCollector == null)
        {
            errors.Add($"{nameof(_runRewardCollector)} is not assigned.");
        }

        if (_obstacleFactory == null)
        {
            errors.Add($"{nameof(_obstacleFactory)} is not assigned.");
        }

        if (_adsConfig == null)
        {
            errors.Add($"{nameof(_adsConfig)} is not assigned.");
        }

        if (errors.Count > 0)
        {
            errorMessage =
                "SceneReferences has invalid configuration:" +
                Environment.NewLine +
                "- " +
                string.Join(Environment.NewLine + "- ", errors);

            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}