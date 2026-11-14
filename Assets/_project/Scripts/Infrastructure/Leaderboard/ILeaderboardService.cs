using System;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public interface ILeaderboardService
    {
        event Action AuthorizationChanged;
        
        bool IsAuthorized { get; }

        void SubmitScore(int score);
        void RequestAuthorization();
    }
}