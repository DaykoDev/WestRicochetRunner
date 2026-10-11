using System;

public interface IPoolable<T>
{
    event Action<T> Died;
    event Action<T> ReturnedToPool;

    void OnReturnedToPool();
}