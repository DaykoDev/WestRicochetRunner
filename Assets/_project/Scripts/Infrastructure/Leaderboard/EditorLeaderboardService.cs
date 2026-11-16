using System;
using YG;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public sealed class EditorLeaderboardService : ILeaderboardService
    {
        private readonly LeaderboardYG _leaderboard;
        private bool _isAuthorized;

        public EditorLeaderboardService(LeaderboardYG leaderboard)
        {
            _leaderboard = leaderboard != null
                ? leaderboard
                : throw new ArgumentNullException(nameof(leaderboard));

            _leaderboard.onUpdateData.AddListener(OnDataUpdated);
        }

        public event Action AuthorizationChanged;
        public event Action DataUpdated;

        public bool IsAuthorized => _isAuthorized;
        
        public void SubmitScore(int score)
        {
        }

        public void Refresh()
        {
            _leaderboard.UpdateLB();
        }

        public void RequestAuthorization()
        {
            _isAuthorized = false;
            AuthorizationChanged?.Invoke();
        }

        private void OnDataUpdated()
        {
            DataUpdated?.Invoke();
        }
    }
}