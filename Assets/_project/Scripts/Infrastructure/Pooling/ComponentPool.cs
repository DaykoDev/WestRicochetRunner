using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace WestRicochet.Infrastructure.Pooling
{
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
            int _maxPooled = 50,
            Action<T> onCreated = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            if (_maxPooled < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(_maxPooled));
            }

            if (capacity > _maxPooled)
            {
                throw new ArgumentException("Capacity cannot exceed maxSize.", nameof(capacity));
            }

            _prefab = prefab;
            _container = container;

            _pool = new ObjectPool<T>(
                createFunc: () =>
                {
                    T instance = UnityEngine.Object.Instantiate(_prefab, _container);
                    onCreated?.Invoke(instance);
                    return instance;
                },
                actionOnGet: instance => instance.gameObject.SetActive(true),
                actionOnRelease: instance =>
                {
                    if (instance.transform.parent != _container)
                    {
                        instance.transform.SetParent(_container);
                    }

                    instance.gameObject.SetActive(false);
                },
                actionOnDestroy: instance => UnityEngine.Object.Destroy(instance.gameObject),
                collectionCheck: true,
                defaultCapacity: capacity,
                maxSize: _maxPooled
            );
        }

        public T Get()
        {
            T instance = _pool.Get();

            _activeObjects.Add(instance);

            if (instance is IPoolable<T> poolable)
            {
                poolable.Removed += Release;
            }

            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null || _activeObjects.Remove(instance) == false)
            {
                return;
            }

            if (instance is IPoolable<T> poolable)
            {
                poolable.Removed -= Release;
                poolable.OnReturnedToPool();
            }

            _pool.Release(instance);
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
                T instance = _activeObjects[i];

                if (instance == null)
                {
                    _activeObjects.RemoveAt(i);
                }
                else
                {
                    Release(instance);
                }
            }
        }
    }
}