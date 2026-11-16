using UnityEngine;

namespace WestRicochet.Infrastructure.Localization
{
    [CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Game/Localization/Database")]
    public sealed class LocalizationDatabase : ScriptableObject
    {
        private const string RussianLanguage = "ru";
        private const string EnglishLanguage = "en";
        private const string TurkishLanguage = "tr";

        [SerializeField] private LocalizationEntry[] _entries;

        public string Get(string key, string language)
        {
            if (_entries == null)
            {
                Debug.LogError(
                    "Localization entries are not assigned.",
                    this);

                return key;
            }

            string normalizedLanguage = NormalizeLanguage(language);

            foreach (LocalizationEntry entry in _entries)
            {
                if (entry == null || entry.Key != key)
                {
                    continue;
                }

                return normalizedLanguage switch
                {
                    RussianLanguage => entry.Ru,
                    TurkishLanguage => entry.Tr,
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
                RussianLanguage => RussianLanguage,
                TurkishLanguage => TurkishLanguage,
                _ => EnglishLanguage
            };
        }
    }
}