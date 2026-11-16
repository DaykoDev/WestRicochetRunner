using System;
using YG;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public sealed class EditorLeaderboardService : ILeaderboardService, IDisposable
    {
        private readonly LeaderboardYG _leaderboard;
        private bool _isAuthorized;
        private bool _isDisposed;

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

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (_leaderboard != null)
            {
                _leaderboard.onUpdateData.RemoveListener(OnDataUpdated);
            }
        }

        private void OnDataUpdated()
        {
            DataUpdated?.Invoke();
        }
    }
}