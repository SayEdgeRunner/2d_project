using System.Collections.Generic;
using Pool;
using UnityEngine;

namespace Core
{
    public class PrefabPoolManager : MonoBehaviour
    {
        private static PrefabPoolManager _instance;

        [Header("풀 설정")]
        [SerializeField] private PoolConfiguration[] _poolConfigurations;

        [Header("컨테이너")]
        [SerializeField] private Transform _poolContainer;

        private static readonly Dictionary<GameObject, GameObjectPool> SharedPrefabToPool = new();
        private static readonly Dictionary<GameObject, GameObject> SharedCloneToPrefab = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (!_poolContainer)
            {
                _poolContainer = transform;
            }

            CreatePools();
        }

        private void CreatePools()
        {
            if (_poolConfigurations == null || _poolConfigurations.Length == 0)
            {
                Debug.LogWarning($"[PrefabPoolManager] No pool configurations defined on {gameObject.name}!");
                return;
            }

            foreach (var config in _poolConfigurations)
            {
                if (!config.Prefab)
                {
                    Debug.LogWarning($"[PrefabPoolManager] Null prefab in configuration on {gameObject.name}, skipping...");
                    continue;
                }

                if (SharedPrefabToPool.ContainsKey(config.Prefab))
                {
                    Debug.LogWarning($"[PrefabPoolManager] Pool for {config.Prefab.name} already exists, skipping duplicate on {gameObject.name}");
                    continue;
                }

                var pool = new GameObjectPool(
                    config.Prefab,
                    _poolContainer,
                    config.InitialCount
                );

                SharedPrefabToPool[config.Prefab] = pool;
            }
        }

        public static GameObject Get(GameObject prefab)
        {
            if (!prefab)
            {
                Debug.LogError("[PrefabPoolManager] Prefab is null!");
                return null;
            }

            if (!SharedPrefabToPool.TryGetValue(prefab, out var pool))
            {
                Debug.LogError($"[PrefabPoolManager] No pool found for prefab: {prefab.name}");
                return null;
            }

            var obj = pool.Get();
            if (obj)
            {
                SharedCloneToPrefab[obj] = prefab;
            }

            return obj;
        }

        public static void Return(GameObject obj)
        {
            if (!obj)
            {
                Debug.LogWarning("[PrefabPoolManager] Cannot return null object to pool");
                return;
            }

            if (!SharedCloneToPrefab.TryGetValue(obj, out var prefab))
            {
                Debug.LogWarning($"[PrefabPoolManager] Object {obj.name} was not obtained from pool");
                return;
            }

            if (!SharedPrefabToPool.TryGetValue(prefab, out var pool))
            {
                Debug.LogWarning($"[PrefabPoolManager] Pool not found for object: {obj.name}");
                return;
            }

            pool.Return(obj);
            SharedCloneToPrefab.Remove(obj);
        }

        public static GameObjectPool GetPool(GameObject prefab)
        {
            return SharedPrefabToPool.GetValueOrDefault(prefab);
        }
    }
}