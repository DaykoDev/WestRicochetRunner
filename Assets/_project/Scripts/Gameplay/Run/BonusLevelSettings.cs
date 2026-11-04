using UnityEngine;

[CreateAssetMenu(fileName = "BonusLevelSettings", menuName = "Game/Run/Bonus Level Settings")]
public sealed class BonusLevelSettings : ScriptableObject
{
    [Tooltip("Каждый N-й уровень является бонусным. 0 отключает бонусные уровни.")]
    [SerializeField, Min(0)] private int _levelInterval = 5;

    [SerializeField] private bool _fillAllCells = true;
    [SerializeField] private bool _secondRevolverEnabled = true;
    [SerializeField] private bool _rainbowBallsEnabled = true;
    [SerializeField, Min(1f)] private float _fireRateMultiplier = 2f;

    public bool SecondRevolverEnabled => _secondRevolverEnabled;
    public bool RainbowBallsEnabled => _rainbowBallsEnabled;
    public float FireRateMultiplier => _fireRateMultiplier;

    public bool IsBonusLevel(int level)
    {
        return level > 0 && _levelInterval > 0 && level % _levelInterval == 0;
    }

    public bool ShouldFillAllCells(int level)
    {
        return level == 1 || (_fillAllCells && IsBonusLevel(level));
    }
}