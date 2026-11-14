using System;
using YG;

namespace WestRicochet.Infrastructure.Leaderboard
{
    public sealed class YandexLeaderboardService : ILeaderboardService, IDisposable
    {
        public event Action AuthorizationChanged;

        public bool IsAuthorized => YG2.isSDKEnabled && YG2.player.auth;

        public YandexLeaderboardService()
        {
            YG2.onGetSDKData += OnGetSDKData;
        }

        public void SubmitScore(int score)
        {
            if (YG2.isSDKEnabled == false)
            {
                return;
            }

            YG2.SetLeaderboard(LeaderboardNames.BestLevel, score);
        }

        public void RequestAuthorization()
        {
            if (YG2.isSDKEnabled == false || IsAuthorized)
            {
                return;
            }

            YG2.OpenAuthDialog();
        }

        public void Dispose()
        {
            YG2.onGetSDKData -= OnGetSDKData;
        }

        private void OnGetSDKData()
        {
            AuthorizationChanged?.Invoke();
        }
    }
}