using System;
using UnityEngine;
using YG;

public sealed class YandexCloudProgressStorage : IProgressStorage, IDisposable
{
    private bool _isSubscribed;
    private bool _isDisposed;

    private Action<PlayerSaveData> _pendingLoad;

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        if (_isDisposed)
        {
            return;
        }

        if (YG2.isSDKEnabled)
        {
            onCompleted?.Invoke(ReadSave());
            return;
        }

        _pendingLoad = onCompleted;
        SubscribeToSdkData();
    }

    public void Save(PlayerSaveData data)
    {
        if (_isDisposed || data == null)
        {
            return;
        }

        YG2.saves.ProgressJson = JsonUtility.ToJson(data);
    }

    public void Flush()
    {
        if (_isDisposed || YG2.isSDKEnabled == false)
        {
            return;
        }

        YG2.SaveProgress();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        UnsubscribeFromSdkData();
        _pendingLoad = null;
    }

    private void SubscribeToSdkData()
    {
        if (_isSubscribed)
        {
            return;
        }

        YG2.onGetSDKData += OnSdkDataReceived;
        _isSubscribed = true;
    }

    private void UnsubscribeFromSdkData()
    {
        if (_isSubscribed == false)
        {
            return;
        }

        YG2.onGetSDKData -= OnSdkDataReceived;
        _isSubscribed = false;
    }

    private void OnSdkDataReceived()
    {
        if (_isDisposed)
        {
            return;
        }

        UnsubscribeFromSdkData();

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