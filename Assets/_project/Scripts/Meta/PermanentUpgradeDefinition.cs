using System;
using UnityEngine;

namespace WestRicochet.Meta
{
    [Serializable]
    public sealed class PermanentUpgradeDefinition
    {
        [SerializeField] private PermanentUpgradeType _type;
        [SerializeField, Min(1)] private int _baseCost = 100;
        [Tooltip("0 means unlimited.")]
        [SerializeField, Min(0)] private int _maxLevel = 10;
        [SerializeField, Min(0.01f)] private float _bonusPerLevel = 1f;

        [Header("Cost")]
        [SerializeField, Min(1f)] private float _costGrowthMultiplier = 1f;

        [Header("Availability")]
        [SerializeField, Min(1)] private int _unlockLevel = 1;

        public PermanentUpgradeType Type => _type;
        public int BaseCost => _baseCost;
        public int MaxLevel => _maxLevel;
        public float BonusPerLevel => _bonusPerLevel;
        public float CostGrowthMultiplier => _costGrowthMultiplier;
        public int UnlockLevel => _unlockLevel;
    }
}