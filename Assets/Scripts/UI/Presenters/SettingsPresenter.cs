using System;

public sealed class SettingsPresenter : IDisposable
{
    private readonly SettingsScreen _screen;
    private readonly SettingsService _settingsService;

    public SettingsPresenter(SettingsScreen screen, SettingsService settingsService)
    {
        _screen = screen;
        _settingsService = settingsService;

        _screen.SoundToggled += OnSoundToggled;
        _screen.MusicToggled += OnMusicToggled;
        _screen.CloseClicked += OnCloseClicked;
        _screen.ExitToMenuClicked += OnExitToMenuClicked;
    }

    public event Action CloseRequested;
    public event Action ExitToMenuRequested;

    public void Show(bool exitToMenuAvailable)
    {
        _screen.SetSoundEnabled(_settingsService.SoundEnabled);
        _screen.SetMusicEnabled(_settingsService.MusicEnabled);
        _screen.Show(exitToMenuAvailable);
    }

    public void Hide()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.SoundToggled -= OnSoundToggled;
        _screen.MusicToggled -= OnMusicToggled;
        _screen.CloseClicked -= OnCloseClicked;
        _screen.ExitToMenuClicked -= OnExitToMenuClicked;
    }

    private void OnSoundToggled(bool enabled) => _settingsService.SetSoundEnabled(enabled);
    private void OnMusicToggled(bool enabled) => _settingsService.SetMusicEnabled(enabled);
    private void OnCloseClicked() => CloseRequested?.Invoke();
    private void OnExitToMenuClicked() => ExitToMenuRequested?.Invoke();
}