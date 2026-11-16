using System;
using YG;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public sealed class YandexLeaderboardService : ILeaderboardService
    {
        private readonly LeaderboardYG _leaderboard;

        public event Action AuthorizationChanged;
        public event Action DataUpdated;

        public bool IsAuthorized => YG2.player.auth;

        public YandexLeaderboardService(LeaderboardYG leaderboard)
        {
            _leaderboard = leaderboard != null
                ? leaderboard
                : throw new ArgumentNullException(nameof(leaderboard));

            _leaderboard.onUpdateData.AddListener(OnDataUpdated);

            YG2.onGetSDKData += OnAuthorizationChanged;
        }

        public void SubmitScore(int score)
        {
            if (IsAuthorized == false)
            {
                return;
            }

            YG2.SetLeaderboard(LeaderboardNames.BestLevel, score);
        }

        public void Refresh()
        {
            int highlightTopCount = _leaderboard.quantityTop;

            if (IsAuthorized == false)
            {
                _leaderboard.quantityTop = _leaderboard.maxQuantityPlayers;
            }

            _leaderboard.UpdateLB();

            _leaderboard.quantityTop = highlightTopCount;
        }

        public void RequestAuthorization()
        {
            YG2.OpenAuthDialog();
        }

        private void OnAuthorizationChanged()
        {
            AuthorizationChanged?.Invoke();
        }

        private void OnDataUpdated()
        {
            DataUpdated?.Invoke();
        }
    }
}