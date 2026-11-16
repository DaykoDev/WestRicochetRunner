using System;
using UnityEngine;

namespace WestRicochet.Infrastructure.Localization
{
    [Serializable]
    public sealed class LocalizationEntry
    {
        [SerializeField] private string _key;

        [SerializeField, TextArea] private string _ru;
        [SerializeField, TextArea] private string _en;
        [SerializeField, TextArea] private string _tr;

        public string Key => _key;
        public string Ru => _ru;
        public string En => _en;
        public string Tr => _tr;
    }
}