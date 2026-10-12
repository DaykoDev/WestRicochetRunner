using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ComponentPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _container;
    private readonly ObjectPool<T> _pool;
    private readonly List<T> _activeObjects = new();

    public ComponentPool(
        T prefab,
        Transform container,
        int capacity = 20,
        int maxSize = 50)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (maxSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSize));
        }

        if (capacity > maxSize)
        {
            throw new ArgumentException(
                "Capacity cannot exceed maxSize.",
                nameof(capacity));
        }

        _prefab = prefab;
        _container = container;

        _pool = new ObjectPool<T>(
            createFunc: () => UnityEngine.Object.Instantiate(_prefab, _container),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj =>
            {
                if (obj.transform.parent != _container)
                {
                    obj.transform.SetParent(_container);
                }

                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: obj => UnityEngine.Object.Destroy(obj.gameObject),
            collectionCheck: true,
            defaultCapacity: capacity,
            maxSize: maxSize
        );
    }

    public T Get()
    {
        T obj = _pool.Get();

        _activeObjects.Add(obj);

        if (obj is IPoolable<T> poolable)
        {
            poolable.Died += Release;
        }

        return obj;
    }

    public void Release(T obj)
    {
        if (obj == null || _activeObjects.Remove(obj) == false)
        {
            return;
        }

        if (obj is IPoolable<T> poolable)
        {
            poolable.Died -= Release;
            poolable.OnReturnedToPool();
        }

        _pool.Release(obj);
    }

    public void Prewarm(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var objects = new List<T>(count);

        for (int i = 0; i < count; i++)
        {
            objects.Add(Get());
        }

        for (int i = 0; i < objects.Count; i++)
        {
            Release(objects[i]);
        }
    }

    public void Reset()
    {
        for (int i = _activeObjects.Count - 1; i >= 0; i--)
        {
            T obj = _activeObjects[i];

            if (obj != null)
            {
                Release(obj);
            }
        }
    }
}