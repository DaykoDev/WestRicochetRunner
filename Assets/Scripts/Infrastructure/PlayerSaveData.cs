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
}