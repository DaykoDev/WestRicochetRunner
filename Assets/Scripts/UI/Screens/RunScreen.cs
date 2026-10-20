using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunScreen : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _coinsText;

    [Header("Health")]
    [SerializeField] private Image[] _healthIcons;
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < _healthIcons.Length; i++)
        {
            Image icon = _healthIcons[i];

            if (icon == null)
            {
                continue;
            }

            icon.gameObject.SetActive(i < maxHealth);

            if (i < maxHealth)
            {
                icon.sprite = i < currentHealth ? _fullHeartSprite : _emptyHeartSprite;
            }
        }
    }

    public void SetLevel(int level)
    {
        if (_levelText != null)
        {
            _levelText.text = $"Level: {level}";
        }
    }

    public void SetCoins(int coins)
    {
        if (_coinsText != null)
        {
            _coinsText.text = $"Coins: {coins}";
        }
    }
}