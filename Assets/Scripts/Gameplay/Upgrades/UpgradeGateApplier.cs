using UnityEngine;

public class UpgradeGateApplier : MonoBehaviour
{
    [SerializeField] private UpgradeGateCharger _charger;

    private RunModifiers _runModifiers;
    private bool _hasApplied;

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _runModifiers = modifiers;
        _hasApplied = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasApplied || _runModifiers == null || _charger == null)
        {
            return;
        }

        if (other.GetComponent<Player>() == null)
        {
            return;
        }

        _runModifiers.Apply(_charger.Type, _charger.CurrentBonus);
        _hasApplied = true;
    }
}