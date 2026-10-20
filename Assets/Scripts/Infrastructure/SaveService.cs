using System;

public class SaveService
{
    private readonly IProgressStorage _storage;

    public SaveService(IProgressStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        _storage.Load(onCompleted);
    }

    public void Save(PlayerSaveData data)
    {
        _storage.Save(data);
    }

    public void Flush()
    {
        _storage.Flush();
    }
}