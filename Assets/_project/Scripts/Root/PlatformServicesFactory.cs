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

    public ILeaderboardService CreateLeaderboardProvider()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexLeaderboardService();
#else
        return new EditorLeaderboardService();
#endif
    }
}