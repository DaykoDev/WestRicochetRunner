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

        public PermanentUpgradeType Type => _type;
        public int BaseCost => _baseCost;
        public int MaxLevel => _maxLevel;
        public float BonusPerLevel => _bonusPerLevel;
    }

    [SerializeField] private Definition[] _definitions;

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