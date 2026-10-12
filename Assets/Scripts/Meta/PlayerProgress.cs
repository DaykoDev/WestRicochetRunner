using System;

public class PlayerProgress
{
    private const int StartingLevel = 1;

    private readonly Random _random = new Random();

    public PlayerProgress()
    {
        CurrentLevel = StartingLevel;
        CurrentLevelSeed = GenerateSeed();
    }

    public int CurrentLevel { get; private set; }
    public int CurrentLevelSeed { get; private set; }

    public void CompleteCurrentLevel()
    {
        CurrentLevel++;
        CurrentLevelSeed = GenerateSeed();
    }

    private int GenerateSeed()
    {
        return _random.Next(1, int.MaxValue);
    }
}