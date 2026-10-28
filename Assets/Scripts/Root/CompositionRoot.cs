using System;
using UnityEngine;

public sealed class CompositionRoot : IDisposable
{
    private readonly SceneReferences _sceneReferences;
    private readonly PlatformServicesFactory _platformFactory;

    private SaveService _saveService;

    private AdsService _adsService;
    private IAdsService _adsProvider;
    private ILeaderboardService _leaderboardService;

    private MetaComposition _meta;
    private RunComposition _run;
    private UiComposition _ui;

    private SettingsFlow _settingsFlow;
    private ProgressSaver _progressSaver;
    private GameEventBindings _eventBindings;

    private bool _isInitializing;
    private bool _isInitialized;
    private bool _isDisposed;

    public CompositionRoot(SceneReferences sceneReferences)
    {
        _sceneReferences = sceneReferences;
        _platformFactory = new PlatformServicesFactory();
    }

    public void Initialize()
    {
        if (_isDisposed || _isInitialized || _isInitializing)
        {
            return;
        }

        if (_sceneReferences == null)
        {
            Debug.LogError("CompositionRoot: SceneReferences is not assigned.");

            return;
        }

        if (_sceneReferences.TryValidate(out string errorMessage) == false)
        {
            Debug.LogError(
                "CompositionRoot initialization failed." +
                Environment.NewLine +
                errorMessage,
                _sceneReferences);

            return;
        }

        _isInitializing = true;

        _saveService = new SaveService(_platformFactory.CreateProgressStorage());

        _saveService.Load(OnProgressLoaded);
    }

    public void FlushSave()
    {
        if (_isDisposed)
        {
            return;
        }

        if (_isInitialized)
        {
            _progressSaver.SaveAndFlush();
        }
        else
        {
            _saveService?.Flush();
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (_isInitialized)
        {
            _progressSaver?.SaveAndFlush();
        }
        else
        {
            _saveService?.Flush();
        }

        _eventBindings?.Dispose();
        _settingsFlow?.Dispose();
        _progressSaver?.Dispose();

        _ui?.Dispose();
        _run?.Dispose();
        _meta?.Dispose();

        _adsService?.Dispose();

        if (_adsProvider is IDisposable disposableAds)
        {
            disposableAds.Dispose();
        }

        _saveService?.Dispose();

        _isInitialized = false;
        _isInitializing = false;
    }

    private void OnProgressLoaded(PlayerSaveData saveData)
    {
        if (_isDisposed || _isInitialized)
        {
            return;
        }

        _isInitializing = false;

        try
        {
            _meta = new MetaComposition(saveData, _sceneReferences);

            _adsProvider = _platformFactory.CreateAdsProvider();

            _leaderboardService = _platformFactory.CreateLeaderboardProvider();

            _adsService = new AdsService(_adsProvider, _sceneReferences.AdsConfig);

            _run = new RunComposition(
                _sceneReferences,
                _meta,
                _adsService);

            _ui = new UiComposition(
                _sceneReferences,
                _meta,
                _run);

            _settingsFlow = new SettingsFlow(
                _ui.Settings,
                _run.PausedState,
                _run.GameFlow);

            _progressSaver = new ProgressSaver(
                _saveService,
                _meta.Progress,
                _meta.Wallet,
                _meta.Settings,
                _run.MainMenuState,
                _run.RunState,
                _run.MenuCoins,
                _run.VictoryMultiplier);

            _eventBindings = new GameEventBindings(
                _leaderboardService,
                _meta,
                _run,
                _ui,
                _settingsFlow);

            _isInitialized = true;

            _run.StateMachine.Enter<BootstrapState>();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, _sceneReferences);

            Dispose();
        }
    }
}