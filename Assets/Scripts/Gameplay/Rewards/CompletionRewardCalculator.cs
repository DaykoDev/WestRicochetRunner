using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CompletionRewardCalculator",
    menuName = "Game/Calculators/Completion Reward")]
public sealed class CompletionRewardCalculator : ScriptableObject
{
    [SerializeField, Min(0)] private int _baseReward = 10;

    [Tooltip("0.8 — плавный рост, 1 — линейный рост.")]
    [SerializeField, Range(0f, 2f)] private float _growth = 0.8f;

    public int Calculate(int completedLevel)
    {
        int level = Math.Max(1, completedLevel);
        double reward = _baseReward * Math.Pow(level, _growth);

        if (double.IsNaN(reward) || double.IsInfinity(reward))
        {
            return int.MaxValue;
        }

        if (reward >= int.MaxValue)
        {
            return int.MaxValue;
        }

        return Math.Max(
            0,
            (int)Math.Round(reward, MidpointRounding.AwayFromZero));
    }
}