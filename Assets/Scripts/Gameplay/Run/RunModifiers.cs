using System;

public class RunModifiers
{
    public int BallCountBonus { get; private set; }
    public int BounceCountBonus { get; private set; }

    public float FireRateBonus { get; private set; }
    public float BallSpeedBonus { get; private set; }

    public float FireRateMultiplier => 1f + FireRateBonus;
    public float BallSpeedMultiplier => 1f + BallSpeedBonus;
    public float IncomeMultiplier { get; private set; } = 1f;

    public event Action Changed;

    public void Apply(UpgradeType type, float value)
    {
        if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            return;

        switch (type)
        {
            case UpgradeType.BallCount:
                BallCountBonus = AddBonus(BallCountBonus, value);
                break;

            case UpgradeType.FireRate:
                FireRateBonus += value / 100f;
                break;

            case UpgradeType.BallSpeed:
                BallSpeedBonus += value / 100f;
                break;

            case UpgradeType.BounceCount:
                BounceCountBonus = AddBonus(BounceCountBonus, value);
                break;

            default:
                return;
        }

        Changed?.Invoke();
    }

    private static int AddBonus(int current, float value)
    {
        long increase = (long)Math.Round(value, MidpointRounding.AwayFromZero);
        return (int)Math.Min(int.MaxValue, (long)current + Math.Max(0L, increase));
    }
}