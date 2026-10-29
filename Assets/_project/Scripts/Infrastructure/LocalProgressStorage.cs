using System;
using UnityEngine;

public sealed class LocalProgressStorage : IProgressStorage
{
    private const string SaveKey = "game.player-progress.v1";

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        PlayerSaveData data = null;

        if (PlayerPrefs.HasKey(SaveKey))
        {
            try
            {
                string json = PlayerPrefs.GetString(SaveKey);

                if (string.IsNullOrWhiteSpace(json) == false)
                {
                    data = JsonUtility.FromJson<PlayerSaveData>(json);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to read local save: {exception.Message}");
            }
        }

        onCompleted?.Invoke(data);
    }

    public void Save(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }

        try
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to prepare local save: {exception.Message}");
        }
    }

    public void Flush()
    {
        PlayerPrefs.Save();
    }
}
