using System;
using UnityEngine;
using UnityEngine.UI;
using WestRicochet.Infrastructure.Localization;
using WestRicochet.Meta;

namespace WestRicochet.UI.Screens
{
    [Serializable]
    public sealed class UpgradePurchaseView
    {
        [SerializeField] private PermanentUpgradeType _type;
        [SerializeField] private Button _button;
        [SerializeField] private LocalizedText _levelText;
        [SerializeField] private LocalizedText _costText;
        [SerializeField] private GameObject _root;

        public PermanentUpgradeType Type => _type;
        public Button Button => _button;
        public LocalizedText LevelText => _levelText;
        public LocalizedText CostText => _costText;
        public GameObject Root => _root;

        public event Action<PermanentUpgradeType> PurchaseRequested;

        public void HandleButtonClicked()
        {
            PurchaseRequested?.Invoke(_type);
        }
    }
}