using System.Collections.Generic;
using UnityEngine;

namespace WestRicochet.Gameplay.Upgrades
{
    public class UpgradeGateGroup : MonoBehaviour
    {
        private readonly HashSet<UpgradeGateApplier> _gates = new();

        private void OnDestroy()
        {
            foreach (UpgradeGateApplier gate in _gates)
            {
                if (gate != null)
                {
                    gate.ClearGroup(this);
                }
            }

            _gates.Clear();
        }

        public void Register(UpgradeGateApplier gate)
        {
            if (gate == null)
            {
                return;
            }

            if (_gates.Add(gate) == false)
            {
                return;
            }

            gate.SetGroup(this);
        }

        public void HandleApplied(UpgradeGateApplier appliedGate)
        {
            if (appliedGate == null)
            {
                return;
            }

            foreach (UpgradeGateApplier gate in _gates)
            {
                if (gate != null && gate != appliedGate)
                {
                    gate.Deactivate();
                }
            }
        }
    }
}