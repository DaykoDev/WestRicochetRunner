using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleHealthCalculator", menuName = "Game/Calculators/Obstacle Health")]
public class ObstacleHealthCalculator : ScriptableObject
{
    [Header("Section Coefficients")]
    [SerializeField, Min(0f)] private float _safeCoefficient = 0.5f;
    [SerializeField, Min(0f)] private float _bonusCoefficient = 0.75f;
    [SerializeField, Min(0f)] private float _challengeCoefficient = 1.5f;

    [Header("Level Growth")]
    [SerializeField, Min(1)] private int _growthChangeLevel = 20;
    [SerializeField, Min(0f)] private float _earlyGrowth = 0.15f;
    [SerializeField, Min(0f)] private float _lateGrowth = 0.1f;

    public int Calculate(int baseHP, int level, ObstacleSection section)
    {
        float sectionCoefficient = GetSectionCoefficient(section);
        float growth = CalculateGrowth(level);

        return Mathf.Max(1, Mathf.RoundToInt(baseHP * sectionCoefficient * growth));
    }

    private float CalculateGrowth(int level)
    {
        level = Mathf.Max(1, level);

        if (level <= _growthChangeLevel)
        {
            return 1f + _earlyGrowth * (level - 1);
        }

        return 1f + _earlyGrowth * (_growthChangeLevel - 1) + _lateGrowth * (level - _growthChangeLevel);
    }

    private float GetSectionCoefficient(ObstacleSection section)
    {
        switch (section)
        {
            case ObstacleSection.Safe:
                return _safeCoefficient;

            case ObstacleSection.Bonus:
                return _bonusCoefficient;

            case ObstacleSection.Challenge:
                return _challengeCoefficient;

            default:
                return 1f;
        }
    }
}