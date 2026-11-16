using WestRicochet.Infrastructure.Ads;
using WestRicochet.Infrastructure.Leaderboard;
using WestRicochet.Infrastructure.Persistence;
using YG;

namespace WestRicochet.Root
{
    public sealed class PlatformServicesFactory
    {
        public IProgressStorage CreateProgressStorage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
            return new YandexCloudProgressStorage();
#else
            return new LocalProgressStorage();
#endif
        }

        public IAdsService CreateAdsProvider()
        {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
            return new YandexAdsService();
#else
            return new EditorAdsService();
#endif
        }

        public ILeaderboardService CreateLeaderboardProvider(LeaderboardYG leaderboard)
        {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
            return new YandexLeaderboardService(leaderboard);
#else
            return new EditorLeaderboardService(leaderboard);
#endif
        }
    }
}