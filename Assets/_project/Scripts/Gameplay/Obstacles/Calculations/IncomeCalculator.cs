using UnityEngine;

[CreateAssetMenu(fileName = "IncomeCalculator", menuName = "Game/Calculators/Income")]
public class IncomeCalculator : ScriptableObject
{
    public int Calculate(int targetHP, float incomeMultiplier, float prefabCoinMultiplier)
    {
        float reward = targetHP * Mathf.Max(0f, incomeMultiplier) * Mathf.Max(0f, prefabCoinMultiplier);

        return Mathf.Max(0, Mathf.RoundToInt(reward));
    }
}