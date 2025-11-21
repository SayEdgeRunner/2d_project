using System.Collections.Generic;
using Core;
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

        [Header("적 타입 및 가중치")]
        [SerializeField] private EnemySpawnData[] _enemySpawnDataList;

        private float _spawnTimer;
        private readonly HashSet<EnemyEntity> _activeEnemies = new();
        private bool _isSpawning;
        private int _totalWeight;

        private void Start()
        {
            if (!_targetTransform)
            {
                Debug.LogWarning($"[EnemySpawner] Target Transform is not assigned on {gameObject.name}");
            }

            if (_enemySpawnDataList == null || _enemySpawnDataList.Length == 0)
            {
                Debug.LogError($"[EnemySpawner] Enemy Spawn Data List is empty on {gameObject.name}");
                return;
            }

            CalculateTotalWeight();

            if (_spawnOnStart)
            {
                StartSpawning();
            }
        }

        private void CalculateTotalWeight()
        {
            _totalWeight = 0;
            foreach (var spawnData in _enemySpawnDataList)
            {
                _totalWeight += spawnData.Weight;
            }
        }

        private void Update()
        {
            if (!_isSpawning) return;

            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer > 0f || !CanSpawn()) return;
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
            return _activeEnemies.Count < _maxEnemyCount;
        }

        private void SpawnEnemy()
        {
            EnemyEntity prefab = SelectRandomEnemyPrefab();
            if (!prefab)
            {
                Debug.LogError($"[EnemySpawner] Failed to select enemy prefab");
                return;
            }

            GameObject obj = PrefabPoolManager.Get(prefab.gameObject);
            if (!obj)
            {
                Debug.LogWarning($"[EnemySpawner] Failed to get enemy from pool: {prefab.name}");
                return;
            }

            Transform spawnTransform = _spawnPoint ? _spawnPoint : transform;
            obj.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);

            EnemyEntity enemy = obj.GetComponent<EnemyEntity>();
            enemy.Initialize(_targetTransform);
            enemy.OnDeath += OnEnemyDeath;

            _activeEnemies.Add(enemy);
        }

        private EnemyEntity SelectRandomEnemyPrefab()
        {
            if (_totalWeight <= 0) return null;

            int randomValue = Random.Range(0, _totalWeight);
            int cumulativeWeight = 0;

            foreach (var spawnData in _enemySpawnDataList)
            {
                cumulativeWeight += spawnData.Weight;
                if (randomValue < cumulativeWeight)
                {
                    return spawnData.EnemyPrefab;
                }
            }

            return null;
        }

        private void OnEnemyDeath(EnemyEntity enemy)
        {
            if (!enemy) return;

            enemy.OnDeath -= OnEnemyDeath;
            _activeEnemies.Remove(enemy);

            PrefabPoolManager.Return(enemy.gameObject);
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
                if (enemy) enemy.OnDeath -= OnEnemyDeath;
            }
        }
    }
}
