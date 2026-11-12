using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

public sealed class LeaderboardScreen : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private LeaderboardYG _leaderboard;

    [Header("Authorization")]
    [SerializeField] private GameObject _authorizationRoot;
    [SerializeField] private Button _authorizationButton;

    [Header("Close")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _outsideButton;

    [Header("Scrolling")]
    [SerializeField] private ScrollRect _scrollRect;

    public event Action AuthorizationClicked;
    public event Action CloseClicked;

    private void OnEnable()
    {
        _authorizationButton.onClick.AddListener(OnAuthorizationClicked);
        _closeButton.onClick.AddListener(OnCloseClicked);
        _outsideButton.onClick.AddListener(OnCloseClicked);

        _leaderboard.onUpdateData.AddListener(OnLeaderboardUpdated);
    }

    private void OnDisable()
    {
        _authorizationButton.onClick.RemoveListener(OnAuthorizationClicked);
        _closeButton.onClick.RemoveListener(OnCloseClicked);
        _outsideButton.onClick.RemoveListener(OnCloseClicked);

        _leaderboard.onUpdateData.RemoveListener(OnLeaderboardUpdated);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshLeaderboard();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetAuthorizationVisible(bool isVisible)
    {
        _authorizationRoot.SetActive(isVisible);
    }

    public void RefreshLeaderboard()
    {
        int highlightTopCount = _leaderboard.quantityTop;

        if (YG2.player.auth == false)
        {
            _leaderboard.quantityTop = _leaderboard.maxQuantityPlayers;
        }

        _leaderboard.UpdateLB();

        _leaderboard.quantityTop = highlightTopCount;
    }

    private void OnAuthorizationClicked()
    {
        AuthorizationClicked?.Invoke();
    }

    private void OnCloseClicked()
    {
        CloseClicked?.Invoke();
    }

    private void OnLeaderboardUpdated()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ScrollToTopNextFrame());
        }
    }

    private IEnumerator ScrollToTopNextFrame()
    {
        yield return null;

        _scrollRect.verticalNormalizedPosition = 1f;
    }
}