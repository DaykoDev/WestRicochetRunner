using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    [SerializeField] private MainMenuScreen _mainMenuScreen;
    [SerializeField] private RunScreen _runScreen;
    [SerializeField] private VictoryScreen _victoryScreen;
    [SerializeField] private DefeatScreen _defeatScreen;
    [SerializeField] private Player _player;
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private TrackScroller _trackScroller;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private PermanentUpgradeSettings _permanentUpgradeSettings;

    public MainMenuScreen MainMenuScreen => _mainMenuScreen;
    public RunScreen RunScreen => _runScreen;
    public VictoryScreen VictoryScreen => _victoryScreen;
    public DefeatScreen DefeatScreen => _defeatScreen;
    public Player Player => _player;
    public LevelGenerator LevelGenerator => _levelGenerator;
    public TrackScroller TrackScroller => _trackScroller;
    public Wallet Wallet => _wallet;
    public PermanentUpgradeSettings PermanentUpgradeSettings => _permanentUpgradeSettings;

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

        if (_player == null)
        {
            errors.Add($"{nameof(_player)} is not assigned.");
        }
        else if (_player.GetComponent<PlayerHealth>() == null)
        {
            errors.Add("Player must have a PlayerHealth component.");
        }

        if (_levelGenerator == null)
        {
            errors.Add($"{nameof(_levelGenerator)} is not assigned.");
        }

        if (_trackScroller == null)
        {
            errors.Add($"{nameof(_trackScroller)} is not assigned.");
        }

        if (_wallet == null)
        {
            errors.Add($"{nameof(_wallet)} is not assigned.");
        }

        if (_permanentUpgradeSettings == null)
        {
            errors.Add($"{nameof(_permanentUpgradeSettings)} is not assigned.");
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