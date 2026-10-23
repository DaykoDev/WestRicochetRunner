public static class ProgressStorageFactory
{
    public static IProgressStorage Create()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        return new YandexCloudProgressStorage();
#else
        return new LocalProgressStorage();
#endif
    }
}
