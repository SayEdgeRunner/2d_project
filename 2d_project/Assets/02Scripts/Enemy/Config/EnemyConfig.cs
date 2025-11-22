using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Game/Enemy Config", order = 0)]
    public class EnemyConfig : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private EEnemyType _enemyType;
        [SerializeField] private GameObject _enemyPrefab;

        [Header("기본 스탯")]
        [SerializeField] private float _health = 100f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private int _experience = 10;
        [SerializeField] private int _score = 10;

        [Header("공격 설정")]
        [Tooltip("이 적이 사용할 공격들")]
        [SerializeField] private BaseEnemyAttackPattern[] _attacks;

        [Header("스폰 설정")]
        [Tooltip("스폰 가중치 - 높을수록 자주 등장")]
        [SerializeField] private int _spawnWeight = 10;
        
        public EEnemyType EnemyType => _enemyType;
        public GameObject EnemyPrefab => _enemyPrefab;
        public float Health => _health;
        public float MoveSpeed => _moveSpeed;
        public int Experience => _experience;
        public int Score => _score;
        public int SpawnWeight => _spawnWeight;

        public BaseEnemyAttackPattern[] Attacks => _attacks;
        public int AttackCount => _attacks?.Length ?? 0;

        public float GetFinalHealth(float difficultyMultiplier = 1f)
        {
            return _health * difficultyMultiplier;
        }

        public float GetFinalMoveSpeed(float difficultyMultiplier = 1f)
        {
            return _moveSpeed * difficultyMultiplier;
        }

        public BaseEnemyAttackPattern GetAttackByIndex(int index)
        {
            if (_attacks == null || index < 0 || index >= _attacks.Length)
                return null;

            return _attacks[index];
        }

        public BaseEnemyAttackPattern GetRandomAttack()
        {
            if (_attacks == null || _attacks.Length == 0)
                return null;

            return _attacks[UnityEngine.Random.Range(0, _attacks.Length)];
        }
    }
}

