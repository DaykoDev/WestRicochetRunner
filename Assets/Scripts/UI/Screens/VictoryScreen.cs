using System;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;

    public event Action MainMenuClicked;

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnMainMenuButtonClicked()
    {
        MainMenuClicked?.Invoke();
    }
}