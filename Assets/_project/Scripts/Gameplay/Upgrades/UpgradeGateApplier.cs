using UnityEngine;
using WestRicochet.Gameplay.Player;
using WestRicochet.Gameplay.Run;

namespace WestRicochet.Gameplay.Upgrades
{
    public class UpgradeGateApplier : MonoBehaviour
    {
        [SerializeField] private UpgradeGateCharger _charger;

        [Header("Gate Root")]
        [SerializeField] private GameObject _gateRoot;

        private RunModifiers _runModifiers;
        private bool _hasApplied;
        private UpgradeGateGroup _group;

        private void Awake()
        {
            if (_gateRoot == null)
            {
                _gateRoot = gameObject;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasApplied || _runModifiers == null || _charger == null)
            {
                return;
            }

            if (other.GetComponentInParent<PlayerController>() == null)
            {
                return;
            }

            _runModifiers.Apply(_charger.Type, _charger.CurrentBonus);
            _hasApplied = true;

            if (_group != null)
            {
                _group.HandleApplied(this);
            }

            Deactivate();
        }

        public void SetRunModifiers(RunModifiers modifiers)
        {
            _runModifiers = modifiers;
            _hasApplied = false;
        }

        public void Deactivate()
        {
            _gateRoot.SetActive(false);
        }

        public void SetGroup(UpgradeGateGroup group)
        {
            _group = group;
        }

        public void ClearGroup(UpgradeGateGroup group)
        {
            if (_group == group)
            {
                _group = null;
            }
        }
    }
}