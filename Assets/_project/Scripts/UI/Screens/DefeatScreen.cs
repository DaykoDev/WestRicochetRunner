using System;
using UnityEngine;
using UnityEngine.UI;

public class DefeatScreen : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _secondChanceButton;

    public event Action MainMenuClicked;
    public event Action RestartClicked;
    public event Action SecondChanceClicked;

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _secondChanceButton.onClick.AddListener(OnSecondChanceClicked);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        _restartButton.onClick.RemoveListener(OnRestartClicked);
        _secondChanceButton.onClick.RemoveListener(OnSecondChanceClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetSecondChanceAvailable(bool isAvailable)
    {
        _secondChanceButton.gameObject.SetActive(isAvailable);
    }

    private void OnMainMenuClicked()
    {
        MainMenuClicked?.Invoke();
    }

    private void OnRestartClicked()
    {
        RestartClicked?.Invoke();
    }

    private void OnSecondChanceClicked()
    {
        SecondChanceClicked?.Invoke();
    }
}