using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Toggle _musicToggle;

    [Header("Buttons")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _exitToMenuButton;

    public event Action<bool> SoundToggled;
    public event Action<bool> MusicToggled;
    public event Action CloseClicked;
    public event Action ExitToMenuClicked;

    private void OnEnable()
    {
        _soundToggle.onValueChanged.AddListener(OnSoundChanged);
        _musicToggle.onValueChanged.AddListener(OnMusicChanged);
        _closeButton.onClick.AddListener(OnClose);
        _exitToMenuButton.onClick.AddListener(OnExitToMenu);
    }

    private void OnDisable()
    {
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

    public void SetSoundEnabled(bool enabled) => _soundToggle.SetIsOnWithoutNotify(enabled);
    public void SetMusicEnabled(bool enabled) => _musicToggle.SetIsOnWithoutNotify(enabled);

    private void OnSoundChanged(bool value) => SoundToggled?.Invoke(value);
    private void OnMusicChanged(bool value) => MusicToggled?.Invoke(value);
    private void OnClose() => CloseClicked?.Invoke();
    private void OnExitToMenu() => ExitToMenuClicked?.Invoke();
}