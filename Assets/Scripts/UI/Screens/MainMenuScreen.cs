using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    public event Action StartClicked;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnStartButtonClicked()
    {
        StartClicked?.Invoke();
    }
}