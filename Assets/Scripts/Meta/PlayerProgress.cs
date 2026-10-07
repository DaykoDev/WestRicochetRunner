using UnityEngine;

public class PlayerProgress
{
    private const int StartingLevel = 1;

    public int CurrentLevel { get; private set; }
    public int CurrentLevelSeed { get; private set; }

    public PlayerProgress()
    {
        CurrentLevel = StartingLevel;
        CurrentLevelSeed = GenerateSeed();
    }

    public void CompleteCurrentLevel()
    {
        CurrentLevel++;
        CurrentLevelSeed = GenerateSeed();
    }

    private int GenerateSeed()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }
}