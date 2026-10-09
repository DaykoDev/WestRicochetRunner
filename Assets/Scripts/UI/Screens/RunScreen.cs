using System;
using UnityEngine;

public class RunScreen : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button _testWinButton;
    [SerializeField] private UnityEngine.UI.Button _testLoseButton;

    public event Action TestWinClicked;
    public event Action TestLoseClicked;

    private void OnEnable()
    {
        _testWinButton.onClick.AddListener(OnTestWinButtonClicked);
        _testLoseButton.onClick.AddListener(OnTestLoseButtonClicked);
    }

    private void OnDisable()
    {
        _testWinButton.onClick.RemoveListener(OnTestWinButtonClicked);
        _testLoseButton.onClick.RemoveListener(OnTestLoseButtonClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnTestWinButtonClicked()
    {
        TestWinClicked?.Invoke();
    }

    private void OnTestLoseButtonClicked()
    {
        TestLoseClicked?.Invoke();
    }
}