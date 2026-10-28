using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuCoinsRewardCalculator", menuName = "Game/Calculators/Menu Coins Reward")]
public sealed class MenuCoinsRewardCalculator : ScriptableObject
{
    [SerializeField, Min(0)] private int _baseCoins = 100;
    [SerializeField, Min(1)] private int _levelsPerStep = 10;
    [SerializeField, Min(0)] private int _coinsPerStep = 100;

    public int Calculate(int currentLevel)
    {
        int level = Math.Max(1, currentLevel);
        int completedSteps = level / _levelsPerStep;

        long reward = _baseCoins + (long)completedSteps * _coinsPerStep;

        return (int)Math.Min(reward, int.MaxValue);
    }
}