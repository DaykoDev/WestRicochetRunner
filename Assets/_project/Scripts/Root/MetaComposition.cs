using System;

public sealed class MetaComposition : IDisposable
{
    public MetaComposition(PlayerSaveData saveData, SceneReferences sceneReferences)
    {
        Progress = new PlayerProgress(saveData);

        Settings = new SettingsService(
            saveData == null || saveData.SoundEnabled,
            saveData == null || saveData.MusicEnabled,
            sceneReferences.AudioMixer);

        Wallet = sceneReferences.Wallet;

        if (Wallet == null)
        {
            throw new InvalidOperationException("Wallet is not assigned in SceneReferences.");
        }

        Wallet.Initialize(saveData?.Coins);

        PermanentUpgradeSettings upgradeSettings = sceneReferences.PermanentUpgradeSettings;

        Upgrades = new UpgradeService(
            Progress,
            Wallet,
            upgradeSettings,
            new UpgradeCostCalculator(upgradeSettings.CostGrowth));
    }

    public PlayerProgress Progress { get; }
    public SettingsService Settings { get; }
    public Wallet Wallet { get; }
    public UpgradeService Upgrades { get; }

    public void Dispose()
    {
        Upgrades.Dispose();
    }
}