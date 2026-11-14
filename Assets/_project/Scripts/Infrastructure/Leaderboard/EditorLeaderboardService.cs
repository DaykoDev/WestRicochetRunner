using System;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public sealed class EditorLeaderboardService : ILeaderboardService
    {
        public bool IsAuthorized => false;

        public event Action AuthorizationChanged;

        public void SubmitScore(int score)
        {
        }

        public void RequestAuthorization()
        {
            AuthorizationChanged?.Invoke();
        }
    }
}