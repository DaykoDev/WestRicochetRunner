using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WestRicochet.Infrastructure.Leaderboard;

namespace WestRicochet.UI.Screens
{
    public sealed class LeaderboardScreen : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _authorizationButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _outsideButton;

        [Header("Views")]
        [SerializeField] private GameObject _authorizationRoot;

        [Header("Scrolling")]
        [SerializeField] private ScrollRect _scrollRect;

        private ILeaderboardService _leaderboardService;
        private Coroutine _scrollCoroutine;

        public event Action AuthorizationClicked;
        public event Action CloseClicked;

        private void OnEnable()
        {
            if (_authorizationButton == null ||
                _closeButton == null ||
                _outsideButton == null)
            {
                Debug.LogError("LeaderboardScreen: All buttons must be assigned.", this);

                enabled = false;
                return;
            }

            if (_leaderboardService == null)
            {
                Debug.LogError("LeaderboardScreen: Initialize was not called.", this);

                enabled = false;
                return;
            }

            _authorizationButton.onClick.AddListener(OnAuthorizationClicked);
            _closeButton.onClick.AddListener(OnCloseClicked);
            _outsideButton.onClick.AddListener(OnCloseClicked);

            _leaderboardService.DataUpdated += OnLeaderboardUpdated;
        }

        private void OnDisable()
        {
            _authorizationButton?.onClick.RemoveListener(OnAuthorizationClicked);
            _closeButton?.onClick.RemoveListener(OnCloseClicked);
            _outsideButton?.onClick.RemoveListener(OnCloseClicked);

            if (_leaderboardService != null)
            {
                _leaderboardService.DataUpdated -= OnLeaderboardUpdated;
            }

            StopScrollCoroutine();
        }

        public void Initialize(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _leaderboardService.Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetAuthorizationVisible(bool isVisible)
        {
            if (_authorizationRoot != null)
            {
                _authorizationRoot.SetActive(isVisible);
            }
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
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }

            if (_scrollRect == null)
            {
                return;
            }

            StopScrollCoroutine();
            _scrollCoroutine = StartCoroutine(ScrollToTopNextFrame());
        }

        private IEnumerator ScrollToTopNextFrame()
        {
            yield return null;

            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = 1f;
            }

            _scrollCoroutine = null;
        }

        private void StopScrollCoroutine()
        {
            if (_scrollCoroutine != null)
            {
                StopCoroutine(_scrollCoroutine);
                _scrollCoroutine = null;
            }
        }
    }
}