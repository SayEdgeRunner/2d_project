using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Enemy
{
    public class EnemyFactory : Singleton<EnemyFactory>
    {
        [Header("적 설정")]
        [SerializeField] private EnemyConfig[] _enemyConfigs;

        private Dictionary<EEnemyType, EnemyConfig> _configMap;

        protected override void Awake()
        {
            base.Awake();
            InitializeConfigs();
        }

        private void InitializeConfigs()
        {
            _configMap = new Dictionary<EEnemyType, EnemyConfig>();

            if (_enemyConfigs == null || _enemyConfigs.Length == 0)
            {
                Debug.LogError("[EnemyFactory] Enemy configs are not assigned!");
                return;
            }

            foreach (var config in _enemyConfigs)
            {
                if (config == null)
                {
                    Debug.LogWarning("[EnemyFactory] Null config found in array");
                    continue;
                }

                if (_configMap.ContainsKey(config.EnemyType))
                {
                    Debug.LogWarning($"[EnemyFactory] Duplicate config for {config.EnemyType}");
                    continue;
                }

                _configMap.Add(config.EnemyType, config);
            }
        }

        public EnemyEntity Create(EEnemyType type, Vector3 position, Transform target, float healthMultiplier = 1f, float moveSpeedMultiplier = 1f)
        {
            if (!_configMap.TryGetValue(type, out EnemyConfig config))
            {
                Debug.LogError($"[EnemyFactory] Config not found for enemy type: {type}");
                return null;
            }

            if (config.EnemyPrefab == null)
            {
                Debug.LogError($"[EnemyFactory] Enemy prefab is null for type: {type}");
                return null;
            }
            
            GameObject obj = PrefabPoolManager.Get(config.EnemyPrefab);
            if (obj == null)
            {
                Debug.LogError($"[EnemyFactory] Failed to get enemy from pool: {type}");
                return null;
            }
            
            obj.transform.position = position;
            
            EnemyEntity enemy = obj.GetComponent<EnemyEntity>();
            if (enemy == null)
            {
                Debug.LogError($"[EnemyFactory] EnemyEntity component not found on {obj.name}");
                PrefabPoolManager.Return(obj);
                return null;
            }
            
            enemy.Initialize(target, config, healthMultiplier, moveSpeedMultiplier);

            return enemy;
        }
        
        public EnemyConfig GetConfig(EEnemyType type)
        {
            if (_configMap.TryGetValue(type, out EnemyConfig config))
            {
                return config;
            }

            Debug.LogWarning($"[EnemyFactory] Config not found for enemy type: {type}.");
            return null;
        }

        public IReadOnlyDictionary<EEnemyType, EnemyConfig> GetAllConfigs()
        {
            return _configMap;
        }

        #if UNITY_EDITOR
        [ContextMenu("Test: Create Police Enemy")]
        private void TestCreatePolice()
        {
            if (Camera.main == null) return;
            Vector3 position = Camera.main.transform.position + Vector3.right * 2f;
            position.z = 0;
            Create(EEnemyType.Police, position, Camera.main.transform);
        }

        [ContextMenu("Test: Create Samurai Enemy")]
        private void TestCreateSamurai()
        {
            if (Camera.main == null) return;
            Vector3 position = Camera.main.transform.position + Vector3.right * 2f;
            position.z = 0;
            Create(EEnemyType.Samurai, position, Camera.main.transform);
        }
        #endif
    }
}
