using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class GameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _container;
        private readonly int _initialCount;

        private readonly Queue<GameObject> _availableObjects = new();
        private readonly HashSet<GameObject> _activeObjects = new();

        public GameObjectPool(GameObject prefab, Transform container, int initialCount)
        {
            _prefab = prefab;
            _container = container;
            _initialCount = initialCount;

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialCount; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject obj = Object.Instantiate(_prefab, _container);
            obj.SetActive(false);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnCreatedInPool();
            }

            _availableObjects.Enqueue(obj);
            return obj;
        }

        public GameObject Get()
        {
            GameObject obj;

            if (_availableObjects.Count > 0)
            {
                obj = _availableObjects.Dequeue();
            }
            else
            {
                // 풀이 비었으면 현재 크기의 150%로 확장 (50% 추가)
                int currentPoolSize = _availableObjects.Count + _activeObjects.Count;
                int expandCount = Mathf.Max(1, Mathf.CeilToInt(currentPoolSize * 0.5f));

                for (int i = 0; i < expandCount; i++)
                {
                    CreateNewObject();
                }

                obj = _availableObjects.Dequeue();
            }

            _activeObjects.Add(obj);
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnGetFromPool();
            }

            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("[GameObjectPool] Trying to return null object to pool");
                return;
            }

            if (!_activeObjects.Contains(obj))
            {
                Debug.LogWarning($"[GameObjectPool] Object {obj.name} is not from this pool or already returned");
                return;
            }

            _activeObjects.Remove(obj);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnReturnToPool();
            }

            obj.SetActive(false);
            _availableObjects.Enqueue(obj);
        }

        public void Clear(bool destroy = true)
        {
            if (destroy)
            {
                foreach (var obj in _availableObjects)
                {
                    if (obj != null)
                    {
                        Object.Destroy(obj);
                    }
                }

                foreach (var obj in _activeObjects)
                {
                    if (obj != null)
                    {
                        Object.Destroy(obj);
                    }
                }
            }

            _availableObjects.Clear();
            _activeObjects.Clear();
        }
    }
}
