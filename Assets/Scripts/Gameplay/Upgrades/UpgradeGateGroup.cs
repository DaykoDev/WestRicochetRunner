using System.Collections.Generic;
using UnityEngine;

public class UpgradeGateGroup : MonoBehaviour
{
    private readonly List<UpgradeGateApplier> _gates = new();

    public void Register(UpgradeGateApplier gate)
    {
        _gates.Add(gate);
        gate.Applied += OnGateApplied;
    }

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