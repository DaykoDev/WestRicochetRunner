using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncomeCalculator", menuName = "Game/Calculators/Income")]
public class IncomeCalculator : ScriptableObject
{
    public void Distribute(IReadOnlyList<Obstacle> obstacles, int budget, float incomeMultiplier)
    {
        if (obstacles == null)
        {
            return;
        }

        float totalWeight = 0f;

        foreach (Obstacle obstacle in obstacles)
        {
            if (obstacle != null)
            {
                totalWeight += Mathf.Max(0f, obstacle.RewardWeight);
            }
        }

        if (totalWeight <= 0f)
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle != null)
                {
                    obstacle.SetIncome(0);
                }
            }

            return;
        }

        double exactReward = Math.Max(0, budget) * (double)Math.Max(0f, incomeMultiplier);

        int totalReward = exactReward >= int.MaxValue ? int.MaxValue : (int)Math.Round(exactReward, MidpointRounding.AwayFromZero);

        float cumulativeWeight = 0f;
        int distributedReward = 0;

        foreach (Obstacle obstacle in obstacles)
        {
            if (obstacle == null)
            {
                continue;
            }

            float weight = Mathf.Max(0f, obstacle.RewardWeight);

            if (weight <= 0f)
            {
                obstacle.SetIncome(0);
                continue;
            }

            cumulativeWeight += weight;

            int targetDistributedReward = Mathf.Clamp(
                Mathf.RoundToInt(totalReward * cumulativeWeight / totalWeight),
                distributedReward,
                totalReward);

            obstacle.SetIncome(targetDistributedReward - distributedReward);
            distributedReward = targetDistributedReward;
        }
    }
}
