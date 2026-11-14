using System;

namespace WestRicochet.Infrastructure.Persistence
{
    public interface IProgressStorage
    {
        void Load(Action<PlayerSaveData> onCompleted);
        void Save(PlayerSaveData data);
        void Flush();
    }
}