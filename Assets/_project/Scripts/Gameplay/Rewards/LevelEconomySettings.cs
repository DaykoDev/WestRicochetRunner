using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEconomySettings", menuName = "Game/Calculators/Level Economy Settings")]
public sealed class LevelEconomySettings : ScriptableObject
{
    [Header("Base Level Budgets")]
    [SerializeField, Min(0)] private int _regularObstacleBudget = 30;
    [SerializeField, Min(0)] private int _completionReward = 10;
    [SerializeField, Min(0)] private int _finalBonusBudget = 30;

    [Header("Growth")]
    [SerializeField, Min(1f)] private float _rewardGrowth = 1.2f;

    [Header("Bonus Level")]
    [SerializeField] private BonusLevelSettings _bonusLevelSettings;
    [SerializeField, Min(1f)] private float _bonusLevelRewardMultiplier = 2f;

    [Header("Menu Advertisement")]
    [Tooltip("Ожидаемый доход обычного уровня, а не сумма полных бюджетов.")]
    [SerializeField, Min(0)] private int _menuBaseReward = 70;

    [SerializeField, Min(0f)] private float _menuRewardMultiplier = 1f;

    public int GetRegularObstacleBudget(int level)
    {
        return CalculateLevelReward(_regularObstacleBudget, level);
    }

    public int GetCompletionReward(int completedLevel)
    {
        return CalculateLevelReward(_completionReward, completedLevel);
    }

    public int GetFinalBonusBudget(int level)
    {
        return CalculateLevelReward(_finalBonusBudget, level);
    }

    public int GetMenuReward(int currentLevel)
    {
        double reward = _menuBaseReward * GetGrowth(currentLevel) * Math.Max(0f, _menuRewardMultiplier);

        return ToInt(reward);
    }

    private int CalculateLevelReward(int baseReward, int level)
    {
        double bonusMultiplier =
            _bonusLevelSettings != null &&
            _bonusLevelSettings.IsBonusLevel(level)
                ? Math.Max(1f, _bonusLevelRewardMultiplier)
                : 1d;

        return ToInt(baseReward * GetGrowth(level) * bonusMultiplier);
    }

    private double GetGrowth(int level)
    {
        int clampedLevel = Math.Max(1, level);
        return Math.Pow(Math.Max(1f, _rewardGrowth), clampedLevel - 1);
    }

    private static int ToInt(double value)
    {
        if (double.IsNaN(value) || value >= int.MaxValue)
        {
            return int.MaxValue;
        }

        if (value <= 0d)
        {
            return 0;
        }

        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
