using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentUpgradeSettings", menuName = "Game/Meta/Permanent Upgrade Settings")]
public class PermanentUpgradeSettings : ScriptableObject
{
    [Serializable]
    public class Definition
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

    [SerializeField] private Definition[] _definitions;

    [Header("Cost Growth")]
    [SerializeField, Min(1f)] private float _costGrowth = 1.25f;

    public float CostGrowth => _costGrowth;

    public bool TryGetDefinition(PermanentUpgradeType type, out Definition definition)
    {
        if (_definitions != null)
        {
            foreach (Definition entry in _definitions)
            {
                if (entry != null && entry.Type == type)
                {
                    definition = entry;
                    return true;
                }
            }
        }

        definition = null;
        return false;
    }
}