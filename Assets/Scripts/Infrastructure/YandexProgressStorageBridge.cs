using System;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
using System.Runtime.InteropServices;
#endif

public sealed class YandexProgressStorageBridge : MonoBehaviour
{
    private const string BridgeObjectName = "YandexProgressStorageBridge";
    private static YandexProgressStorageBridge _instance;

    private Action<string, string> _loadCompleted;

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
    [DllImport("__Internal")] private static extern void YG_LoadProgress(string gameObjectName);
    [DllImport("__Internal")] private static extern void YG_SaveProgress(string gameObjectName, string json, int flushImmediately);
    [DllImport("__Internal")] private static extern void YG_FlushProgress(string gameObjectName);
#endif

    public static YandexProgressStorageBridge GetOrCreate()
    {
        if (_instance != null)
            return _instance;

        _instance = FindObjectOfType<YandexProgressStorageBridge>();
        if (_instance != null)
            return _instance;

        GameObject bridgeObject = new GameObject(BridgeObjectName);
        _instance = bridgeObject.AddComponent<YandexProgressStorageBridge>();
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        gameObject.name = BridgeObjectName;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(Action<string, string> onCompleted)
    {
        _loadCompleted = onCompleted;

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        YG_LoadProgress(gameObject.name);
#else
        CompleteLoad(string.Empty, "Yandex cloud storage is only enabled in a Yandex WebGL build.");
#endif
    }

    public void SaveData(string json, bool flushImmediately)
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        YG_SaveProgress(gameObject.name, json, flushImmediately ? 1 : 0);
#endif
    }

    public void FlushPendingSave()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        YG_FlushProgress(gameObject.name);
#endif
    }

    // Called by the .jslib bridge via Unity SendMessage.
    public void OnCloudLoadCompleted(string json)
    {
        CompleteLoad(json, null);
    }

    // Called by the .jslib bridge via Unity SendMessage.
    public void OnCloudLoadFailed(string error)
    {
        CompleteLoad(null, error);
    }

    // Called by the .jslib bridge via Unity SendMessage.
    public void OnCloudSaveCompleted(string result)
    {
        if (!string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase))
            Debug.LogWarning($"Yandex cloud save failed: {result}");
    }

    private void CompleteLoad(string json, string error)
    {
        Action<string, string> callback = _loadCompleted;
        _loadCompleted = null;
        callback?.Invoke(json, error);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
