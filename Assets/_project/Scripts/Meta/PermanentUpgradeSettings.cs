using UnityEngine;

namespace WestRicochet.Meta
{
    [CreateAssetMenu(fileName = "PermanentUpgradeSettings", menuName = "Game/Meta/Permanent Upgrade Settings")]
    public class PermanentUpgradeSettings : ScriptableObject
    {
        [SerializeField] private PermanentUpgradeDefinition[] _definitions;

        [Header("Cost Growth")]
        [SerializeField, Min(1f)] private float _costGrowth = 1.25f;

        public float CostGrowth => _costGrowth;

        public bool TryGetDefinition(PermanentUpgradeType type, out PermanentUpgradeDefinition definition)
        {
            if (_definitions != null)
            {
                foreach (PermanentUpgradeDefinition entry in _definitions)
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
}