using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    [Header("Toggles")]
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Toggle _musicToggle;

    [Header("Buttons")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _exitToMenuButton;

    [Header("Sound Icons")]
    [SerializeField] private GameObject _soundOnIcon;
    [SerializeField] private GameObject _soundOffIcon;

    [Header("Music Icons")]
    [SerializeField] private GameObject _musicOnIcon;
    [SerializeField] private GameObject _musicOffIcon;

    public event Action<float> SoundVolumeChanged;
    public event Action<float> MusicVolumeChanged;
    public event Action<bool> SoundToggled;
    public event Action<bool> MusicToggled;
    public event Action CloseClicked;
    public event Action ExitToMenuClicked;

    private void OnEnable()
    {
        _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _soundToggle.onValueChanged.AddListener(OnSoundChanged);
        _musicToggle.onValueChanged.AddListener(OnMusicChanged);
        _closeButton.onClick.AddListener(OnClose);
        _exitToMenuButton.onClick.AddListener(OnExitToMenu);
    }

    private void OnDisable()
    {
        _soundVolumeSlider.onValueChanged.RemoveListener(OnSoundVolumeChanged);
        _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
        _musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        _closeButton.onClick.RemoveListener(OnClose);
        _exitToMenuButton.onClick.RemoveListener(OnExitToMenu);
    }

    public void Show(bool exitToMenuAvailable)
    {
        _exitToMenuButton.gameObject.SetActive(exitToMenuAvailable);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetSoundVolume(float volume) => _soundVolumeSlider.SetValueWithoutNotify(volume);
    public void SetMusicVolume(float volume) => _musicVolumeSlider.SetValueWithoutNotify(volume);

    public void SetSoundEnabled(bool enabled)
    {
        _soundToggle.SetIsOnWithoutNotify(enabled);
        UpdateSoundIcons(enabled);
    }

    public void SetMusicEnabled(bool enabled)
    {
        _musicToggle.SetIsOnWithoutNotify(enabled);
        UpdateMusicIcons(enabled);
    }

    private void OnSoundVolumeChanged(float volume) => SoundVolumeChanged?.Invoke(volume);
    private void OnMusicVolumeChanged(float volume) => MusicVolumeChanged?.Invoke(volume);

    private void OnSoundChanged(bool value)
    {
        UpdateSoundIcons(value);
        SoundToggled?.Invoke(value);
    }

    private void OnMusicChanged(bool value)
    {
        UpdateMusicIcons(value);
        MusicToggled?.Invoke(value);
    }

    private void UpdateSoundIcons(bool enabled)
    {
        _soundOnIcon.SetActive(enabled);
        _soundOffIcon.SetActive(!enabled);
    }

    private void UpdateMusicIcons(bool enabled)
    {
        _musicOnIcon.SetActive(enabled);
        _musicOffIcon.SetActive(!enabled);
    }

    private void OnClose() => CloseClicked?.Invoke();
    private void OnExitToMenu() => ExitToMenuClicked?.Invoke();
}