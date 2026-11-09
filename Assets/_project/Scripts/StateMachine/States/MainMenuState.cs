using System;
using UnityEngine;
using Cinemachine;

public class MainMenuState : IState
{
    private readonly RunState _runState;
    private readonly UpgradeService _upgradeService;

    public MainMenuState(RunState runState, UpgradeService upgradeService)
    {
        _runState = runState;
        _upgradeService = upgradeService;
    }

    public event Action Entered;
    public event Action Exited;

    public void Enter()
    {
        Time.timeScale = 1f;

        CinemachineImpulseManager.Instance.Clear();

        _upgradeService.UpgradeLevelChanged += OnUpgradeLevelChanged;

        PreparePreview();

        Entered?.Invoke();
    }

    public void Exit()
    {
        _upgradeService.UpgradeLevelChanged -= OnUpgradeLevelChanged;

        Exited?.Invoke();
    }

    private void OnUpgradeLevelChanged(PermanentUpgradeType type, int level)
    {
        _runState.ClearRun();
        PreparePreview();
    }

    private void PreparePreview()
    {
        if (_runState.PrepareRun() == false)
        {
            Debug.LogError("Failed to prepare main menu level preview.");
        }
    }
}