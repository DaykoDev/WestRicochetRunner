using System;

public sealed class UiComposition : IDisposable
{
    public UiComposition(SceneReferences sceneReferences, MetaComposition meta, RunComposition run)
    {
        MainMenu = new MainMenuPresenter(
            sceneReferences.MainMenuScreen,
            meta.Upgrades,
            run.MenuCoins);

        Run = new RunPresenter(
            sceneReferences.RunScreen,
            sceneReferences.PlayerHealth,
            meta.Progress,
            sceneReferences.RunRewardCollector,
            sceneReferences.TrackScroller,
            sceneReferences.LevelGenerator,
            sceneReferences.BallQueue);

        Victory = new VictoryPresenter(
            sceneReferences.VictoryScreen,
            sceneReferences.RunRewardCollector,
            run.VictoryMultiplier,
            meta.Wallet);

        Defeat = new DefeatPresenter(
            sceneReferences.DefeatScreen,
            run.SecondChance);

        Settings = new SettingsPresenter(
            sceneReferences.SettingsScreen,
            meta.Settings);
    }

    public MainMenuPresenter MainMenu { get; }
    public RunPresenter Run { get; }
    public VictoryPresenter Victory { get; }
    public DefeatPresenter Defeat { get; }
    public SettingsPresenter Settings { get; }

    public void Dispose()
    {
        Settings.Dispose();
        Defeat.Dispose();
        Victory.Dispose();
        Run.Dispose();
        MainMenu.Dispose();
    }
}