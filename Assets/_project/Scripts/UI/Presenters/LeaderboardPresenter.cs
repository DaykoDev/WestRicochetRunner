using System;
using WestRicochet.Infrastructure.Leaderboard;
using WestRicochet.UI.Screens;

namespace WestRicochet.UI.Presenters
{
    public sealed class LeaderboardPresenter : IDisposable
    {
        private readonly LeaderboardScreen _screen;
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardPresenter(LeaderboardScreen screen, ILeaderboardService leaderboardService)
        {
            _screen = screen;
            _leaderboardService = leaderboardService;

            _screen.Initialize(_leaderboardService);

            _screen.CloseClicked += OnCloseClicked;
            _screen.AuthorizationClicked += OnAuthorizationClicked;

            _leaderboardService.AuthorizationChanged += OnAuthorizationChanged;
        }

        public void Show()
        {
            RefreshAuthorization();
            _screen.Show();
        }

        public void Hide()
        {
            _screen.Hide();
        }

        public void Dispose()
        {
            _screen.CloseClicked -= OnCloseClicked;
            _screen.AuthorizationClicked -= OnAuthorizationClicked;

            _leaderboardService.AuthorizationChanged -= OnAuthorizationChanged;
        }

        private void RefreshAuthorization()
        {
            _screen.SetAuthorizationVisible(_leaderboardService.IsAuthorized == false);
        }

        private void OnAuthorizationClicked()
        {
            _leaderboardService.RequestAuthorization();
        }

        private void OnAuthorizationChanged()
        {
            RefreshAuthorization();

            if (_screen.gameObject.activeSelf)
            {
                _leaderboardService.Refresh();
            }
        }

        private void OnCloseClicked()
        {
            Hide();
        }
    }
}