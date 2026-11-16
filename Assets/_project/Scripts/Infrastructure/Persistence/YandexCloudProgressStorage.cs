using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace WestRicochet.Infrastructure.Persistence
{
    public sealed class YandexCloudProgressStorage : IProgressStorage, IDisposable
    {
        private readonly List<Action<PlayerSaveData>> _pendingLoads = new();

        private bool _isSubscribed;
        private bool _isDisposed;

        

        public void Load(Action<PlayerSaveData> onCompleted)
        {
            if (_isDisposed || onCompleted == null)
            {
                return;
            }

            if (YG2.isSDKEnabled)
            {
                onCompleted.Invoke(ReadSave());
                return;
            }

            _pendingLoads.Add(onCompleted);
            SubscribeToSdkData();
        }

        public void Save(PlayerSaveData data)
        {
            if (_isDisposed || data == null)
            {
                return;
            }

            if (YG2.isSDKEnabled == false)
            {
                Debug.LogWarning(
                    "YandexCloudProgressStorage: SDK is not enabled, " +
                    "cloud save skipped.");

                return;
            }

            if (YG2.saves == null)
            {
                Debug.LogError(
                    "YandexCloudProgressStorage: YG2.saves is not " +
                    "available.");

                return;
            }

            try
            {
                YG2.saves.ProgressJson = JsonUtility.ToJson(data);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"YandexCloudProgressStorage: failed to serialize " +
                    $"save data. {exception.Message}");
            }
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
            _pendingLoads.Clear();
        }

        private PlayerSaveData ReadSave()
        {
            try
            {
                string json = YG2.saves?.ProgressJson;

                if (string.IsNullOrEmpty(json))
                {
                    return new PlayerSaveData();
                }

                return JsonUtility.FromJson<PlayerSaveData>(json)
                       ?? new PlayerSaveData();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"YandexCloudProgressStorage: failed to deserialize " +
                    $"save data. {exception.Message}");

                return new PlayerSaveData();
            }
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
            UnsubscribeFromSdkData();

            List<Action<PlayerSaveData>> callbacks = new(_pendingLoads);
            _pendingLoads.Clear();

            PlayerSaveData data = ReadSave();

            foreach (Action<PlayerSaveData> callback in callbacks)
            {
                callback?.Invoke(data);
            }
        }
    }
}