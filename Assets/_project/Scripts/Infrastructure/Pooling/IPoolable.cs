using System;

namespace WestRicochet.Infrastructure.Pooling
{
    public interface IPoolable<T>
    {
        event Action<T> Removed;
        event Action<T> ReturnedToPool;

        void OnReturnedToPool();
    }
}