using UnityEngine;
using TMPro;

public sealed class RainbowTextGroup : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float _saturation = 0.75f;

    [SerializeField, Min(0f)]
    private float _hueOffsetPerText = 0.08f;

    private TMP_Text[] _texts;
    private Color[] _defaultColors;

    private bool _isEnabled;
    private float _hue;

    private void Awake()
    {
        RefreshTexts();
    }

    private void Update()
    {
        if (_isEnabled == false || _texts == null)
        {
            return;
        }

        _hue = Mathf.Repeat(_hue + _speed * Time.deltaTime, 1f);

        for (int i = 0; i < _texts.Length; i++)
        {
            TMP_Text text = _texts[i];

            if (text == null)
            {
                continue;
            }

            float textHue = Mathf.Repeat(_hue + i * _hueOffsetPerText, 1f);

            text.color = Color.HSVToRGB(textHue, _saturation, 1f);
        }
    }

    private void OnDisable()
    {
        SetEnabled(false);
    }

    public void SetEnabled(bool isEnabled)
    {
        if (_isEnabled == isEnabled)
        {
            return;
        }

        _isEnabled = isEnabled;

        if (_isEnabled == false)
        {
            RestoreDefaultColors();
        }
    }

    public void RefreshTexts()
    {
        _texts = GetComponentsInChildren<TMP_Text>(true);
        _defaultColors = new Color[_texts.Length];

        for (int i = 0; i < _texts.Length; i++)
        {
            _defaultColors[i] = _texts[i].color;
        }
    }

    private void RestoreDefaultColors()
    {
        if (_texts == null || _defaultColors == null)
        {
            return;
        }

        for (int i = 0; i < _texts.Length; i++)
        {
            if (_texts[i] != null)
            {
                _texts[i].color = _defaultColors[i];
            }
        }
    }
}