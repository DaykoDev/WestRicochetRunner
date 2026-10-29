using System;

public class PlayerProgress
{
    private const int StartingLevel = 1;
    private readonly Random _random = new();

    private int _startBallCountUpgradeLevel;
    private int _ballDamageUpgradeLevel;
    private int _incomeMultiplierUpgradeLevel;
    private bool _isTutorialCompleted;
    private int _bestCompletedLevel;

    public PlayerProgress() : this(null) { }

    public PlayerProgress(PlayerSaveData saveData)
    {
        if (saveData == null)
        {
            CurrentLevel = StartingLevel;
            CurrentLevelSeed = GenerateSeed();
            return;
        }

        CurrentLevel = Math.Max(StartingLevel, saveData.CurrentLevel);
        CurrentLevelSeed = saveData.CurrentLevelSeed > 0 ? saveData.CurrentLevelSeed : GenerateSeed();

        _startBallCountUpgradeLevel = Math.Max(0, saveData.StartBallCountUpgradeLevel);
        _ballDamageUpgradeLevel = Math.Max(0, saveData.BallDamageUpgradeLevel);
        _incomeMultiplierUpgradeLevel = Math.Max(0, saveData.IncomeMultiplierUpgradeLevel);
        _isTutorialCompleted = saveData.TutorialCompleted;
        _bestCompletedLevel = Math.Max(0, saveData.BestCompletedLevel);
    }

    public int CurrentLevel { get; private set; }
    public int CurrentLevelSeed { get; private set; }

    public bool IsTutorialCompleted => _isTutorialCompleted;
    public int BestCompletedLevel => _bestCompletedLevel;

    public event Action<int> BestLevelChanged;
    public event Action TutorialCompleted;
    public event Action<int> LevelChanged;
    public event Action<PermanentUpgradeType, int> UpgradeLevelChanged;

    public void CompleteTutorial()
    {
        if (_isTutorialCompleted)
        {
            return;
        }

        _isTutorialCompleted = true;
        TutorialCompleted?.Invoke();
    }

    public void CompleteCurrentLevel()
    {
        if (CurrentLevel < int.MaxValue)
        {
            CurrentLevel++;
        }

        int completedLevel = CurrentLevel - 1;

        if (completedLevel > _bestCompletedLevel)
        {
            _bestCompletedLevel = completedLevel;
            BestLevelChanged?.Invoke(_bestCompletedLevel);
        }

        CurrentLevelSeed = GenerateSeed();
        LevelChanged?.Invoke(CurrentLevel);
    }

    public int GetUpgradeLevel(PermanentUpgradeType type)
    {
        switch (type)
        {
            case PermanentUpgradeType.StartBallCount:
                return _startBallCountUpgradeLevel;
            case PermanentUpgradeType.BallDamage:
                return _ballDamageUpgradeLevel;
            case PermanentUpgradeType.IncomeMultiplier:
                return _incomeMultiplierUpgradeLevel;
            default:
                return 0;
        }
    }

    public bool TryIncreaseUpgradeLevel(PermanentUpgradeType type)
    {
        int currentLevel = GetUpgradeLevel(type);

        if (currentLevel == int.MaxValue)
        {
            return false;
        }

        int newLevel = currentLevel + 1;

        switch (type)
        {
            case PermanentUpgradeType.StartBallCount:
                _startBallCountUpgradeLevel = newLevel;
                break;
            case PermanentUpgradeType.BallDamage:
                _ballDamageUpgradeLevel = newLevel;
                break;
            case PermanentUpgradeType.IncomeMultiplier:
                _incomeMultiplierUpgradeLevel = newLevel;
                break;
            default:
                return false;
        }

        UpgradeLevelChanged?.Invoke(type, newLevel);

        return true;
    }

    public PlayerSaveData CreateSaveData(int coins)
    {
        return new PlayerSaveData
        {
            CurrentLevel = CurrentLevel,
            CurrentLevelSeed = CurrentLevelSeed,
            Coins = Math.Max(0, coins),
            StartBallCountUpgradeLevel = _startBallCountUpgradeLevel,
            BallDamageUpgradeLevel = _ballDamageUpgradeLevel,
            IncomeMultiplierUpgradeLevel = _incomeMultiplierUpgradeLevel,
            TutorialCompleted = _isTutorialCompleted,
            BestCompletedLevel = _bestCompletedLevel
        };
    }

    public void ResetProgress()
    {
        CurrentLevel = StartingLevel;
        CurrentLevelSeed = GenerateSeed();

        _startBallCountUpgradeLevel = 0;
        _ballDamageUpgradeLevel = 0;
        _incomeMultiplierUpgradeLevel = 0;

        _isTutorialCompleted = false;
        _bestCompletedLevel = 0;

        BestLevelChanged?.Invoke(_bestCompletedLevel);
        LevelChanged?.Invoke(CurrentLevel);

        UpgradeLevelChanged?.Invoke(
            PermanentUpgradeType.StartBallCount,
            _startBallCountUpgradeLevel);

        UpgradeLevelChanged?.Invoke(
            PermanentUpgradeType.BallDamage,
            _ballDamageUpgradeLevel);

        UpgradeLevelChanged?.Invoke(
            PermanentUpgradeType.IncomeMultiplier,
            _incomeMultiplierUpgradeLevel);
    }

    private int GenerateSeed()
    {
        return _random.Next(1, int.MaxValue);
    }
}