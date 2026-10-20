using System;
using UnityEngine;

public sealed class YandexCloudProgressStorage : IProgressStorage
{
    private readonly IProgressStorage _localStorage;
    private YandexProgressStorageBridge _bridge;
    private PlayerSaveData _localFallbackData;

    public YandexCloudProgressStorage(IProgressStorage localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        if (onCompleted == null)
        {
            return;
        }

        _localStorage.Load(localData => _localFallbackData = localData);

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        _bridge = YandexProgressStorageBridge.GetOrCreate();
        _bridge.LoadData((json, error) =>
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    PlayerSaveData cloudData = JsonUtility.FromJson<PlayerSaveData>(json);
                    if (cloudData != null)
                    {
                        _localStorage.Save(cloudData);
                        _localStorage.Flush();
                        onCompleted(cloudData);
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Cloud save could not be parsed; using local save. {exception.Message}");
                }
            }

            bool cloudHadNoSave = string.IsNullOrWhiteSpace(error) && string.IsNullOrWhiteSpace(json);

            if (cloudHadNoSave && _localFallbackData != null)
            {
                // Migrate an existing local save to the cloud if this Yandex player has no cloud data yet.
                _bridge.SaveData(JsonUtility.ToJson(_localFallbackData), false);
            }
            else if (!string.IsNullOrWhiteSpace(error))
            {
                Debug.LogWarning($"Yandex cloud load failed; using local save. {error}");
            }

            onCompleted(_localFallbackData);
        });
#else
        onCompleted(_localFallbackData);
#endif
    }

    public void Save(PlayerSaveData data)
    {
        if (data == null)
            return;

        // Update the local backup immediately; the cloud adapter batches frequent balance changes.
        _localStorage.Save(data);

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        if (_bridge == null)
            _bridge = YandexProgressStorageBridge.GetOrCreate();

        _bridge.SaveData(JsonUtility.ToJson(data), false);
#endif
    }

    public void Flush()
    {
        _localStorage.Flush();

#if UNITY_WEBGL && !UNITY_EDITOR && YANDEX_GAMES
        if (_bridge != null)
            _bridge.FlushPendingSave();
#endif
    }
}
