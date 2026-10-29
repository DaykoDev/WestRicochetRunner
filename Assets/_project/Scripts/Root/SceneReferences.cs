using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SceneReferences : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MainMenuScreen _mainMenuScreen;
    [SerializeField] private RunScreen _runScreen;
    [SerializeField] private VictoryScreen _victoryScreen;
    [SerializeField] private DefeatScreen _defeatScreen;
    [SerializeField] private SettingsScreen _settingsScreen;

    [Header("Player")]
    [SerializeField] private Player _player;
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Gameplay")]
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private TrackScroller _trackScroller;
    [SerializeField] private BallQueue _ballQueue;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private PermanentUpgradeSettings _permanentUpgradeSettings;
    [SerializeField] private RunRewardCollector _runRewardCollector;
    [SerializeField] private ObstacleFactory _obstacleFactory;

    [Header("Services")]
    [SerializeField] private AdsConfig _adsConfig;

    [SerializeField] private AudioMixer _audioMixer;

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

    public AudioMixer AudioMixer => _audioMixer;

    public bool TryValidate(out string errorMessage)
    {
        List<string> errors = new List<string>();

        ValidateReference(_mainMenuScreen, nameof(_mainMenuScreen), errors);
        ValidateReference(_runScreen, nameof(_runScreen), errors);
        ValidateReference(_victoryScreen, nameof(_victoryScreen), errors);
        ValidateReference(_defeatScreen, nameof(_defeatScreen), errors);
        ValidateReference(_settingsScreen, nameof(_settingsScreen), errors);

        ValidateReference(_player, nameof(_player), errors);
        ValidateReference(_playerHealth, nameof(_playerHealth), errors);

        ValidateReference(_levelGenerator, nameof(_levelGenerator), errors);
        ValidateReference(_trackScroller, nameof(_trackScroller), errors);
        ValidateReference(_ballQueue, nameof(_ballQueue), errors);
        ValidateReference(_wallet, nameof(_wallet), errors);
        ValidateReference(_permanentUpgradeSettings, nameof(_permanentUpgradeSettings), errors);
        ValidateReference(_runRewardCollector, nameof(_runRewardCollector), errors);
        ValidateReference(_obstacleFactory, nameof(_obstacleFactory), errors);

        ValidateReference(_adsConfig, nameof(_adsConfig), errors);

        ValidateReference(_audioMixer, nameof(_audioMixer), errors);

        if (errors.Count == 0)
        {
            errorMessage = string.Empty;
            return true;
        }

        errorMessage = "SceneReferences has invalid configuration:" + Environment.NewLine + "- " + string.Join(Environment.NewLine + "- ", errors);

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