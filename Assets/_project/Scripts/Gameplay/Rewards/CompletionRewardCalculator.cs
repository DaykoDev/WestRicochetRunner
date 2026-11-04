using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CompletionRewardCalculator", menuName = "Game/Calculators/Completion Reward")]
public sealed class CompletionRewardCalculator : ScriptableObject
{
    [SerializeField] private LevelEconomySettings _levelEconomySettings;

    public int Calculate(int completedLevel)
    {
        if (_levelEconomySettings == null)
        {
            Debug.LogError("LevelEconomySettings is not assigned.", this);
            return 0;
        }

        return _levelEconomySettings.GetCompletionReward(completedLevel);
    }
}