using DG.Tweening;
using System;

public sealed class ProgressSaver : IDisposable
{
    private const float SettingsSaveDelay = 0.3f;

    private readonly SaveService _saveService;
    private readonly PlayerProgress _playerProgress;
    private readonly Wallet _wallet;
    private readonly SettingsService _settingsService;
    private readonly MainMenuState _mainMenuState;
    private readonly RunState _runState;
    private readonly RewardedCoinsFlow _coinsFlow;
    private readonly RewardedMultiplierFlow _multiplierFlow;

    private bool _isDisposed;
    private Tween _settingsSaveTween;

    public ProgressSaver(
        SaveService saveService,
        PlayerProgress playerProgress,
        Wallet wallet,
        SettingsService settingsService,
        MainMenuState mainMenuState,
        RunState runState,
        RewardedCoinsFlow coinsFlow,
        RewardedMultiplierFlow multiplierFlow)
    {
        _saveService = saveService;
        _playerProgress = playerProgress;
        _wallet = wallet;
        _settingsService = settingsService;
        _mainMenuState = mainMenuState;
        _runState = runState;
        _coinsFlow = coinsFlow;
        _multiplierFlow = multiplierFlow;

        Subscribe();
    }

    public void SaveAndFlush()
    {
        if (_isDisposed)
        {
            return;
        }

        PlayerSaveData data = _playerProgress.CreateSaveData(_wallet.Balance);

        data.SoundVolume = _settingsService.SoundVolume;
        data.MusicVolume = _settingsService.MusicVolume;
        data.SoundEnabled = _settingsService.SoundEnabled;
        data.MusicEnabled = _settingsService.MusicEnabled;

        _saveService.Save(data);
        _saveService.Flush();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _settingsSaveTween?.Kill();
        _settingsSaveTween = null;

        Unsubscribe();
    }

    private void Subscribe()
    {
        _playerProgress.LevelChanged += OnProgressChanged;
        _playerProgress.UpgradeLevelChanged += OnUpgradeLevelChanged;
        _playerProgress.TutorialCompleted += OnTutorialCompleted;

        _settingsService.SoundVolumeChanged += OnVolumeChanged;
        _settingsService.MusicVolumeChanged += OnVolumeChanged;
        _settingsService.SoundEnabledChanged += OnSettingChanged;
        _settingsService.MusicEnabledChanged += OnSettingChanged;

        _runState.Lost += OnRunLost;
        _mainMenuState.Entered += OnMainMenuEntered;

        _coinsFlow.RewardGranted += OnRewardGranted;
        _multiplierFlow.BonusGranted += OnRewardGranted;
    }

    private void Unsubscribe()
    {
        _playerProgress.LevelChanged -= OnProgressChanged;
        _playerProgress.UpgradeLevelChanged -= OnUpgradeLevelChanged;
        _playerProgress.TutorialCompleted -= OnTutorialCompleted;

        _settingsService.SoundVolumeChanged -= OnVolumeChanged;
        _settingsService.MusicVolumeChanged -= OnVolumeChanged;
        _settingsService.SoundEnabledChanged -= OnSettingChanged;
        _settingsService.MusicEnabledChanged -= OnSettingChanged;

        _runState.Lost -= OnRunLost;
        _mainMenuState.Entered -= OnMainMenuEntered;

        _coinsFlow.RewardGranted -= OnRewardGranted;
        _multiplierFlow.BonusGranted -= OnRewardGranted;
    }

    private void OnProgressChanged(int level)
    {
        SaveAndFlush();
    }

    private void OnUpgradeLevelChanged(PermanentUpgradeType type, int level)
    {
        SaveAndFlush();
    }

    private void OnTutorialCompleted()
    {
        SaveAndFlush();
    }

    private void OnVolumeChanged(float volume)
    {
        ScheduleSettingsSave();
    }

    private void ScheduleSettingsSave()
    {
        _settingsSaveTween?.Kill();

        _settingsSaveTween = DOVirtual.DelayedCall(
                SettingsSaveDelay,
                () =>
                {
                    _settingsSaveTween = null;
                    SaveAndFlush();
                })
            .SetUpdate(true);
    }

    private void OnSettingChanged(bool isEnabled)
    {
        SaveAndFlush();
    }

    private void OnRunLost()
    {
        SaveAndFlush();
    }

    private void OnMainMenuEntered()
    {
        SaveAndFlush();
    }

    private void OnRewardGranted(int amount)
    {
        SaveAndFlush();
    }
}