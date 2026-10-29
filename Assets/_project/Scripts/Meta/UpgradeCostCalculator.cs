using System;

public class UpgradeCostCalculator
{
    private readonly double _costGrowth;

    public UpgradeCostCalculator(double costGrowth)
    {
        _costGrowth = Math.Max(1d, costGrowth);
    }

    public int CalculateNextCost(PermanentUpgradeSettings.Definition definition, int currentLevel)
    {
        if (definition == null)
        {
            return 0;
        }

        double cost = definition.BaseCost * Math.Pow(_costGrowth, Math.Max(0, currentLevel));

        if (double.IsNaN(cost) || double.IsInfinity(cost) || cost >= int.MaxValue)
        {
            return int.MaxValue;
        }

        return Math.Max(1, (int)Math.Round(cost, MidpointRounding.AwayFromZero));
    }
}