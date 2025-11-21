using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("스폰 설정")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private int _maxEnemyCount = 5;
        [SerializeField] private bool _spawnOnStart = true;

        [Header("참조")]
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private EnemyFactory _enemyFactory;

        [Header("스폰할 적 타입 (비어있으면 모든 타입)")]
        [SerializeField] private EEnemyType[] _spawnableTypes;

        private float _spawnTimer;
        private readonly HashSet<EnemyEntity> _activeEnemies = new();
        private bool _isSpawning;
        private List<EnemyConfig> _spawnConfigs;
        private int _totalWeight;

        private void Start()
        {
            if (!_targetTransform)
            {
                Debug.LogWarning($"[EnemySpawner] Target Transform is not assigned on {gameObject.name}");
            }

            if (!_enemyFactory)
            {
                Debug.LogError($"[EnemySpawner] Enemy Factory is not assigned on {gameObject.name}");
                return;
            }

            InitializeSpawnConfigs();

            if (_spawnOnStart)
            {
                StartSpawning();
            }
        }

        private void InitializeSpawnConfigs()
        {
            var allConfigs = _enemyFactory.GetAllConfigs();

            if (_spawnableTypes != null && _spawnableTypes.Length > 0)
            {
                List<EnemyConfig> spawnConfigs = new List<EnemyConfig>();
                foreach (var type in _spawnableTypes)
                {
                    if (allConfigs.ContainsKey(type)) spawnConfigs.Add(allConfigs[type]);
                }

                _spawnConfigs = spawnConfigs;
            }
            else
            {
                _spawnConfigs = allConfigs.Values.ToList();
            }

            if (_spawnConfigs.Count == 0)
            {
                Debug.LogError($"[EnemySpawner] No spawn configs available on {gameObject.name}");
                return;
            }

            CalculateTotalWeight();
        }

        private void CalculateTotalWeight()
        {
            _totalWeight = 0;
            foreach (var config in _spawnConfigs)
            {
                _totalWeight += config.SpawnWeight;
            }
        }

        private void Update()
        {
            if (!_isSpawning) return;

            _spawnTimer -= Time.deltaTime;

            if (!CanSpawn()) return;
            SpawnEnemy();
            _spawnTimer = _spawnInterval;
        }

        private void StartSpawning()
        {
            _isSpawning = true;
            _spawnTimer = 0f;
        }

        public void StopSpawning()
        {
            _isSpawning = false;
        }

        public void SetSpawnInterval(float interval)
        {
            _spawnInterval = Mathf.Max(0.1f, interval);
        }

        private bool CanSpawn()
        {
            return _spawnTimer <= 0f && _activeEnemies.Count < _maxEnemyCount;
        }

        private void SpawnEnemy()
        {
            EEnemyType selectedType = SelectRandomEnemyType();

            Transform spawnTransform = _spawnPoint ? _spawnPoint : transform;

            EnemyEntity enemy = _enemyFactory.Create(selectedType, spawnTransform.position, _targetTransform);

            if (!enemy)
            {
                Debug.LogWarning($"[EnemySpawner] Failed to create enemy: {selectedType}");
                return;
            }

            enemy.OnDeathComplete += OnEnemyDeath;
            _activeEnemies.Add(enemy);
        }

        private EEnemyType SelectRandomEnemyType()
        {
            if (_totalWeight <= 0 || _spawnConfigs.Count == 0)
            {
                Debug.LogError("[EnemySpawner] Cannot select enemy type: invalid weight or configs");
                return EEnemyType.Police;
            }

            int randomValue = Random.Range(0, _totalWeight);
            int cumulativeWeight = 0;

            foreach (var config in _spawnConfigs)
            {
                cumulativeWeight += config.SpawnWeight;
                if (randomValue < cumulativeWeight)
                {
                    return config.EnemyType;
                }
            }
            
            return _spawnConfigs[0].EnemyType;
        }

        private void OnEnemyDeath(EnemyEntity enemy)
        {
            if (!enemy) return;

            enemy.OnDeathComplete -= OnEnemyDeath;
            _activeEnemies.Remove(enemy);

            Core.PrefabPoolManager.Return(enemy.gameObject);
        }

        private void OnDrawGizmos()
        {
            if (!_spawnPoint) return;

            Gizmos.color = _isSpawning ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_spawnPoint.position, 0.5f);

            #if UNITY_EDITOR
            UnityEditor.Handles.Label(
                _spawnPoint.position + Vector3.up,
                $"Active: {_activeEnemies.Count}/{_maxEnemyCount}"
            );
            #endif
        }

        private void OnDestroy()
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy) enemy.OnDeathComplete -= OnEnemyDeath;
            }
        }
    }
}
