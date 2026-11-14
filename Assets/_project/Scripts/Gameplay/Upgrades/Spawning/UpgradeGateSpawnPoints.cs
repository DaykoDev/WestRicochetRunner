using System.Collections.Generic;
using UnityEngine;

namespace WestRicochet.Gameplay.Upgrades.Spawning
{
    public class UpgradeGateSpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] _points;

        public IReadOnlyList<Transform> Points => _points;
    }
}