using System;
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

        public event Action<UpgradeGateApplier> Applied;

        private void Awake()
        {
            if (_gateRoot == null)
            {
                _gateRoot = gameObject;
            }
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

            Applied?.Invoke(this);
            Deactivate();
        }
    }
}