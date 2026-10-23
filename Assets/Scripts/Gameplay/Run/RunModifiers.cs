using System;

public class RunModifiers
{
    public int BallCountBonus { get; private set; }
    public int BounceCountBonus { get; private set; }

    public float FireRateBonus { get; private set; }
    public float BallSpeedBonus { get; private set; }
    public int BallDamageBonus { get; private set; }

    public float FireRateMultiplier => 1f + FireRateBonus;
    public float BallSpeedMultiplier => 1f + BallSpeedBonus;
    public float IncomeMultiplier { get; private set; } = 1f;

    public event Action Changed;

    public void Apply(UpgradeType type, float value)
    {
        if (IsValidPositive(value) == false)
        {
            return;
        }

        switch (type)
        {
            case UpgradeType.BallCount:
                BallCountBonus = AddBonus(BallCountBonus, value);
                break;
            case UpgradeType.FireRate:
                FireRateBonus = AddPositive(FireRateBonus, value / 100f);
                break;
            case UpgradeType.BallSpeed:
                BallSpeedBonus = AddPositive(BallSpeedBonus, value / 100f);
                break;
            case UpgradeType.BounceCount:
                BounceCountBonus = AddBonus(BounceCountBonus, value);
                break;
            default:
                return;
        }

        Changed?.Invoke();
    }

    public void AddBallDamageBonus(int value)
    {
        if (value <= 0)
        {
            return;
        }

        BallDamageBonus = AddBonus(BallDamageBonus, value);
        Changed?.Invoke();
    }

    public void AddIncomeMultiplierBonus(float value)
    {
        if (IsValidPositive(value) == false)
        {
            return;
        }

        IncomeMultiplier = AddPositive(IncomeMultiplier, value);
        Changed?.Invoke();
    }

    private static bool IsValidPositive(float value)
    {
        return value > 0f && !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static float AddPositive(float current, float value)
    {
        double result = (double)current + value;

        return result >= float.MaxValue ? float.MaxValue : (float)result;
    }

    private static int AddBonus(int current, float value)
    {
        double increase = Math.Round(value, MidpointRounding.AwayFromZero);

        if (increase <= 0d)
        {
            return current;
        }

        if (increase >= int.MaxValue - (double)current)
        {
            return int.MaxValue;
        }

        return current + (int)increase;
    }
}