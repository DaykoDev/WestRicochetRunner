using System;
using System.Collections.Generic;

public sealed class RunComposition : IDisposable
{
    public RunComposition(SceneReferences sceneReferences, MetaComposition meta, AdsService adsService)
    {
        SecondChance = new SecondChanceFlow(
            adsService,
            sceneReferences.ObstacleFactory,
            sceneReferences.Player,
            sceneReferences.PlayerHealth,
            sceneReferences.AdsConfig.SecondChanceClearRadius);

        VictoryMultiplier = new RewardedMultiplierFlow(
            adsService,
            meta.Wallet,
            sceneReferences.RunRewardCollector,
            sceneReferences.AdsConfig.VictoryCoinsMultiplier);

        MenuCoins = new RewardedCoinsFlow(
            adsService,
            meta.Wallet,
            meta.Progress,
            sceneReferences.AdsConfig.MenuCoinsRewardCalculator);

        BootstrapState = new BootstrapState();
        MainMenuState = new MainMenuState();

        RunState = new RunState(
            meta.Progress,
            sceneReferences.Player,
            sceneReferences.PlayerHealth,
            sceneReferences.LevelGenerator,
            sceneReferences.TrackScroller,
            meta.Upgrades);

        VictoryState = new VictoryState();
        DefeatState = new DefeatState();
        PausedState = new PausedState();

        List<IState> states = new()
        {
            BootstrapState,
            MainMenuState,
            RunState,
            VictoryState,
            DefeatState,
            PausedState
        };

        StateMachine = new GameStateMachine(states);

        GameFlow = new GameFlow(
            StateMachine,
            meta.Progress,
            sceneReferences.PlayerHealth,
            sceneReferences.LevelGenerator,
            RunState,
            SecondChance,
            adsService,
            meta.Wallet,
            sceneReferences.RunRewardCollector);
    }

    public SecondChanceFlow SecondChance { get; }

    public RewardedMultiplierFlow VictoryMultiplier { get; }

    public RewardedCoinsFlow MenuCoins { get; }

    public BootstrapState BootstrapState { get; }
    public MainMenuState MainMenuState { get; }
    public RunState RunState { get; }
    public VictoryState VictoryState { get; }
    public DefeatState DefeatState { get; }
    public PausedState PausedState { get; }

    public GameStateMachine StateMachine { get; }
    public GameFlow GameFlow { get; }

    public void Dispose()
    {
        GameFlow?.Dispose();

        SecondChance?.Dispose();
        VictoryMultiplier?.Dispose();
        MenuCoins?.Dispose();
    }
}