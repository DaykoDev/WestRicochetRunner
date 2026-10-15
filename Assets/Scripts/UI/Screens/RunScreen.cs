using System;
using UnityEngine;
using UnityEngine.UI;

public class RunScreen : MonoBehaviour
{
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
            if (icon == null) continue;

            icon.gameObject.SetActive(i < maxHealth);

            if (i < maxHealth)
                icon.sprite = i < currentHealth ? _fullHeartSprite : _emptyHeartSprite;
        }
    }
}