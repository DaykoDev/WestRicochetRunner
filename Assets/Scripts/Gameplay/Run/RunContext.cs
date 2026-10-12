public class RunContext
{
    public RunContext(int level, int seed, RunModifiers modifiers)
    {
        Level = level;
        Seed = seed;
        Modifiers = modifiers;
    }

    public int Level { get; }
    public int Seed { get; }
    public RunModifiers Modifiers { get; }
    public int CollectedCoins { get; private set; }
    public bool SecondChanceUsed { get; private set; }

    public void AddCoins(int amount)
    {
        CollectedCoins += amount;
    }

    public void UseSecondChance()
    {
        SecondChanceUsed = true;
    }
}