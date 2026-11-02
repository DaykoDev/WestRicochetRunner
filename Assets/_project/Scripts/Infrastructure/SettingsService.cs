using System;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SettingsService
{
    private const float DisabledVolume = -80f;
    private const float MinAudibleVolume = 0.0001f;

    private readonly AudioMixer _audioMixer;

    private bool _soundEnabled;
    private bool _musicEnabled;

    private float _soundVolume;
    private float _musicVolume;

    public SettingsService(bool soundEnabled, bool musicEnabled, float soundVolume, float musicVolume, AudioMixer audioMixer)
    {
        _soundEnabled = soundEnabled;
        _musicEnabled = musicEnabled;

        _soundVolume = Mathf.Clamp01(soundVolume);
        _musicVolume = Mathf.Clamp01(musicVolume);

        _audioMixer = audioMixer;

        ApplyAudio();
    }

    public bool SoundEnabled => _soundEnabled;
    public bool MusicEnabled => _musicEnabled;

    public float SoundVolume => _soundVolume;
    public float MusicVolume => _musicVolume;

    public event Action<bool> SoundEnabledChanged;
    public event Action<bool> MusicEnabledChanged;

    public event Action<float> SoundVolumeChanged;
    public event Action<float> MusicVolumeChanged;

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

    public void SetSoundVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (Mathf.Approximately(_soundVolume, volume))
        {
            return;
        }

        _soundVolume = volume;

        ApplyAudio();
        SoundVolumeChanged?.Invoke(volume);
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (Mathf.Approximately(_musicVolume, volume))
        {
            return;
        }

        _musicVolume = volume;

        ApplyAudio();
        MusicVolumeChanged?.Invoke(volume);
    }

    private void ApplyAudio()
    {
        if (_audioMixer == null)
        {
            return;
        }

        float soundVolume = _soundEnabled ? VolumeToDecibels(_soundVolume) : DisabledVolume;

        float musicVolume = _musicEnabled ? VolumeToDecibels(_musicVolume) : DisabledVolume;

        _audioMixer.SetFloat("FXVolume", soundVolume);
        _audioMixer.SetFloat("MusicVolume", musicVolume);
    }

    private static float VolumeToDecibels(float volume)
    {
        if (volume <= MinAudibleVolume)
        {
            return DisabledVolume;
        }

        return Mathf.Log10(volume) * 20f;
    }
}