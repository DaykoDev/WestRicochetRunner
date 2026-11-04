using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuCoinsRewardCalculator", menuName = "Game/Calculators/Menu Coins Reward")]
public sealed class MenuCoinsRewardCalculator : ScriptableObject
{
    [SerializeField] private LevelEconomySettings _levelEconomySettings;

    public int Calculate(int currentLevel)
    {
        if (_levelEconomySettings == null)
        {
            Debug.LogError("LevelEconomySettings is not assigned.", this);
            return 0;
        }

        return _levelEconomySettings.GetMenuReward(currentLevel);
    }
}