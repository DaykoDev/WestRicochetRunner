using System;
using UnityEngine;
using YG;

public sealed class YandexCloudProgressStorage : IProgressStorage
{
    private Action<PlayerSaveData> _pendingLoad;

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        if (YG2.isSDKEnabled)
        {
            onCompleted?.Invoke(ReadSave());
            return;
        }

        _pendingLoad = onCompleted;
        YG2.onGetSDKData += OnSdkDataReceived;
    }

    public void Save(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }

        YG2.saves.ProgressJson = JsonUtility.ToJson(data);

        if (YG2.isSDKEnabled)
        {
            YG2.SaveProgress();
        }
    }

    public void Flush()
    {
        if (YG2.isSDKEnabled)
        {
            YG2.SaveProgress();
        }
    }

    private void OnSdkDataReceived()
    {
        YG2.onGetSDKData -= OnSdkDataReceived;

        Action<PlayerSaveData> callback = _pendingLoad;
        _pendingLoad = null;
        callback?.Invoke(ReadSave());
    }

    private PlayerSaveData ReadSave()
    {
        string json = YG2.saves.ProgressJson;

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to read cloud save: {exception.Message}");
            return null;
        }
    }
}