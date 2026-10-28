using System;

public class SaveService : IDisposable
{
    private readonly IProgressStorage _storage;

    private bool _isDisposed;

    public SaveService(IProgressStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public void Load(Action<PlayerSaveData> onCompleted)
    {
        if (_isDisposed)
        {
            return;
        }

        _storage.Load(onCompleted);
    }

    public void Save(PlayerSaveData data)
    {
        if (_isDisposed)
        {
            return;
        }

        _storage.Save(data);
    }

    public void Flush()
    {
        if (_isDisposed)
        {
            return;
        }

        _storage.Flush();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (_storage is IDisposable disposableStorage)
        {
            disposableStorage.Dispose();
        }
    }
}