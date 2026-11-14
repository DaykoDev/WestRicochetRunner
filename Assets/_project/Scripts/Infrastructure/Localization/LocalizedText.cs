using TMPro;
using UnityEngine;
using YG;

namespace WestRicochet.Infrastructure.Localization
{
    public sealed class LocalizedText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private LocalizationDatabase _database;
        [SerializeField] private string _key;

        private object[] _arguments;

        private string _plainText;
        private bool _hasPlainText;

        private void Awake()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }
        }

        private void OnEnable()
        {
            YG2.onSwitchLang += OnLanguageChanged;
            YG2.onCorrectLang += OnLanguageChanged;

            Apply();
        }

        private void OnDisable()
        {
            YG2.onSwitchLang -= OnLanguageChanged;
            YG2.onCorrectLang -= OnLanguageChanged;
        }

        public void SetKey(string key, params object[] arguments)
        {
            _key = key;
            _arguments = arguments;
            _hasPlainText = false;

            Apply();
        }

        public void SetPlainText(string text)
        {
            _plainText = text;
            _hasPlainText = true;

            Apply();
        }

        private void OnLanguageChanged(string language)
        {
            Apply();
        }

        private void Apply()
        {
            if (_text == null)
            {
                return;
            }

            if (_hasPlainText)
            {
                _text.text = _plainText;
                return;
            }

            if (_database == null || string.IsNullOrEmpty(_key))
            {
                return;
            }

            string template = _database.Get(_key, YG2.lang);

            _text.text = _arguments == null || _arguments.Length == 0
                ? template
                : string.Format(template, _arguments);
        }
    }
}