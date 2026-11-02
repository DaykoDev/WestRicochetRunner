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
    public int BounceCountUpgradeLevel;

    public bool TutorialCompleted;

    public int BestCompletedLevel;

    public bool SoundEnabled = true;
    public bool MusicEnabled = true;
    public float SoundVolume = 1f;
    public float MusicVolume = 1f;
}