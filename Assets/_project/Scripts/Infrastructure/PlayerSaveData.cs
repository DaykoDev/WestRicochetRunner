using System;

[Serializable]
public class PlayerSaveData
{
    public int CurrentLevel = 1;
    public int CurrentLevelSeed;
    public int Coins;
    public int StartBallCountUpgradeLevel;
    public int BallDamageUpgradeLevel;
    public int IncomeMultiplierUpgradeLevel;

    public bool TutorialCompleted;

    public int BestCompletedLevel;

    public bool SoundEnabled = true;
    public bool MusicEnabled = true;

    public static PlayerSaveData CreateDefault()
    {
        return new PlayerSaveData
        {
            CurrentLevel = 1,
            Coins = 0,
            StartBallCountUpgradeLevel = 1,
            BallDamageUpgradeLevel = 1,
            IncomeMultiplierUpgradeLevel = 1
        };
    }
}