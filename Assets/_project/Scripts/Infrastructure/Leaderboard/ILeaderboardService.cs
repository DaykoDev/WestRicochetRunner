using System;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public interface ILeaderboardService
    {
        bool IsAuthorized { get; }

        event Action AuthorizationChanged;
        event Action DataUpdated;

        void SubmitScore(int score);
        void Refresh();
        void RequestAuthorization();

    }
}