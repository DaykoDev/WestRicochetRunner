using System;

public class UpgradeCostCalculator
{
    private const double CostGrowth = 1.25d;

    public int CalculateNextCost(PermanentUpgradeSettings.Definition definition, int currentLevel)
    {
        if (definition == null)
        {
            return 0;
        }

        double cost = definition.BaseCost * Math.Pow(CostGrowth, Math.Max(0, currentLevel));

        if (double.IsNaN(cost) || double.IsInfinity(cost) || cost >= int.MaxValue)
        {
            return int.MaxValue;
        }

        return Math.Max(1, (int)Math.Round(cost, MidpointRounding.AwayFromZero));
    }
}