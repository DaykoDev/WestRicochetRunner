using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Game/Localization/Database")]
public sealed class LocalizationDatabase : ScriptableObject
{
    [Serializable]
    private sealed class Entry
    {
        [SerializeField] private string _key;

        [SerializeField, TextArea]
        private string _ru;

        [SerializeField, TextArea]
        private string _en;

        [SerializeField, TextArea]
        private string _tr;

        public string Key => _key;
        public string Ru => _ru;
        public string En => _en;
        public string Tr => _tr;
    }

    [SerializeField] private Entry[] _entries;

    public string Get(string key, string language)
    {
        string normalizedLanguage = NormalizeLanguage(language);

        foreach (Entry entry in _entries)
        {
            if (entry == null || entry.Key != key)
            {
                continue;
            }

            return normalizedLanguage switch
            {
                "ru" => entry.Ru,
                "tr" => entry.Tr,
                _ => entry.En
            };
        }

        Debug.LogWarning($"Localization key is missing: {key}", this);

        return key;
    }

    private static string NormalizeLanguage(string language)
    {
        return language switch
        {
            "ru" => "ru",
            "tr" => "tr",
            _ => "en"
        };
    }
}