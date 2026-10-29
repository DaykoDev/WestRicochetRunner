using System;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SettingsService
{
    private const float EnabledVolume = 0f;
    private const float DisabledVolume = -80f;

    private readonly AudioMixer _audioMixer;

    private bool _soundEnabled;
    private bool _musicEnabled;

    public SettingsService(bool soundEnabled, bool musicEnabled, AudioMixer audioMixer)
    {
        _soundEnabled = soundEnabled;
        _musicEnabled = musicEnabled;
        _audioMixer = audioMixer;
        ApplyAudio();
    }

    public bool SoundEnabled => _soundEnabled;
    public bool MusicEnabled => _musicEnabled;

    public event Action<bool> SoundEnabledChanged;
    public event Action<bool> MusicEnabledChanged;

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
        ApplyAudio();
        MusicEnabledChanged?.Invoke(enabled);
    }

    private void ApplyAudio()
    {
        if (_audioMixer == null)
        {
            return;
        }

        _audioMixer.SetFloat("FXVolume", _soundEnabled ? EnabledVolume : DisabledVolume);
        _audioMixer.SetFloat("MusicVolume", _musicEnabled ? EnabledVolume : DisabledVolume);
    }
}