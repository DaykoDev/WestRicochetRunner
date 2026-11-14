using System.Collections.Generic;
using UnityEngine;

namespace WestRicochet.Gameplay.Upgrades
{
    public class UpgradeGateGroup : MonoBehaviour
    {
        private readonly List<UpgradeGateApplier> _gates = new();

        private void OnDestroy()
        {
            foreach (UpgradeGateApplier gate in _gates)
            {
                if (gate != null)
                {
                    gate.Applied -= OnGateApplied;
                }
            }

            _gates.Clear();
        }

        public void Register(UpgradeGateApplier gate)
        {
            _gates.Add(gate);
            gate.Applied += OnGateApplied;
        }

        private void OnGateApplied(UpgradeGateApplier appliedGate)
        {
            foreach (UpgradeGateApplier gate in _gates)
            {
                if (gate != appliedGate)
                {
                    gate.Deactivate();
                }
            }
        }
    }
}