using System;

public interface IProgressStorage
{
    void Load(Action<PlayerSaveData> onCompleted);
    void Save(PlayerSaveData data);
    void Flush();
}
