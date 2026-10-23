using System;
using UnityEngine;

public sealed class SettingsService
{
    private const float EnabledVolume = 1f;
    private const float DisabledVolume = 0f;

    private bool _soundEnabled;
    private bool _musicEnabled;
    private bool _vibrationEnabled;

    public SettingsService(bool soundEnabled, bool musicEnabled, bool vibrationEnabled)
    {
        _soundEnabled = soundEnabled;
        _musicEnabled = musicEnabled;
        _vibrationEnabled = vibrationEnabled;
        ApplyAudio();
    }

    public bool SoundEnabled => _soundEnabled;
    public bool MusicEnabled => _musicEnabled;
    public bool VibrationEnabled => _vibrationEnabled;

    public event Action<bool> SoundEnabledChanged;
    public event Action<bool> MusicEnabledChanged;
    public event Action<bool> VibrationEnabledChanged;

    public void SetSoundEnabled(bool enabled)
    {
        if (_soundEnabled == enabled)
        {
            return;
        }

        _soundEnabled = enabled;
        ApplyAudio();
        SoundEnabledChanged?.Invoke(enabled);
    }

    public void SetMusicEnabled(bool enabled)
    {
        if (_musicEnabled == enabled)
        {
            return;
        }

        _musicEnabled = enabled;
        MusicEnabledChanged?.Invoke(enabled);
    }

    public void SetVibrationEnabled(bool enabled)
    {
        if (_vibrationEnabled == enabled)
        {
            return;
        }

        _vibrationEnabled = enabled;
        VibrationEnabledChanged?.Invoke(enabled);
    }

    private void ApplyAudio()
    {
        AudioListener.volume = _soundEnabled ? EnabledVolume : DisabledVolume;
    }
}