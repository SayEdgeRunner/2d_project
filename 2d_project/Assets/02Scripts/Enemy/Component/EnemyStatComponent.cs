using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyStatComponent : MonoBehaviour
    {
        private float _baseHealth;
        private float _baseMoveSpeed;
        private int _experience;
        private int _score;
        
        
        [Header("Debug")]
        [SerializeField] private List<BaseEnemyAttackPattern> _attacks;

        [Header("현재 스탯")]
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _moveSpeed;

        public float BaseHealth => _baseHealth;
        public float BaseMoveSpeed => _baseMoveSpeed;
        public int Experience => _experience;
        public int Score => _score;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;

        public void Initialize(EnemyConfig config, float healthMultiplier = 1f, float moveSpeedMultiplier = 1f)
        {
            if (config == null)
            {
                Debug.LogError("[EnemyStatComponent] Config is null!");
                return;
            }

            _baseHealth = config.Health;
            _baseMoveSpeed = config.MoveSpeed;
            _experience = config.Experience;
            _score = config.Score;

            _maxHealth = config.GetFinalHealth(healthMultiplier);
            _moveSpeed = config.GetFinalMoveSpeed(moveSpeedMultiplier);

            _attacks = new List<BaseEnemyAttackPattern>();
            if (config.Attacks != null)
            {
                _attacks.AddRange(config.Attacks);
            }
        }
        
        public void ModifyMoveSpeed(float multiplier)
        {
            _moveSpeed = _baseMoveSpeed * multiplier;
        }
        
        public void ModifyMaxHealth(float multiplier)
        {
            _maxHealth = _baseHealth * multiplier;
        }
        
        public void AddMoveSpeed(float amount)
        {
            _moveSpeed += amount;
            if (_moveSpeed < 0) _moveSpeed = 0;
        }
        
        public void AddMaxHealth(float amount)
        {
            _maxHealth += amount;
            if (_maxHealth < 0) _maxHealth = 0;
        }
        
        public void ResetStats()
        {
            _maxHealth = _baseHealth;
            _moveSpeed = _baseMoveSpeed;
        }
        
        public BaseEnemyAttackPattern[] GetAllAttacks()
        {
            if (_attacks == null) return System.Array.Empty<BaseEnemyAttackPattern>();
            return _attacks.ToArray();
        }

        public BaseEnemyAttackPattern GetAttackByIndex(int index)
        {
            if (_attacks == null || index < 0 || index >= _attacks.Count)
                return null;

            return _attacks[index];
        }

        public BaseEnemyAttackPattern GetRandomAttack()
        {
            if (_attacks == null || _attacks.Count == 0)
                return null;

            return _attacks[Random.Range(0, _attacks.Count)];
        }

        public int GetAttackCount()
        {
            return _attacks?.Count ?? 0;
        }

        public bool HasAttacks()
        {
            return _attacks != null && _attacks.Count > 0;
        }

        public T GetAttackByType<T>() where T : BaseEnemyAttackPattern
        {
            if (_attacks == null)
                return null;

            return _attacks.Find(attack => attack is T) as T;
        }

        public BaseEnemyAttackPattern[] GetAttacksByType<T>() where T : BaseEnemyAttackPattern
        {
            if (_attacks == null)
                return System.Array.Empty<BaseEnemyAttackPattern>();

            return _attacks.FindAll(attack => attack is T).ToArray();
        }

#if UNITY_EDITOR
        [ContextMenu("Test: Increase Move Speed x1.5")]
        private void TestIncreaseMoveSpeed()
        {
            ModifyMoveSpeed(1.5f);
            Debug.Log($"Move Speed increased to: {_moveSpeed}");
        }

        [ContextMenu("Test: Decrease Move Speed x0.5")]
        private void TestDecreaseMoveSpeed()
        {
            ModifyMoveSpeed(0.5f);
            Debug.Log($"Move Speed decreased to: {_moveSpeed}");
        }

        [ContextMenu("Test: Double Max Health")]
        private void TestDoubleMaxHealth()
        {
            ModifyMaxHealth(2f);
            Debug.Log($"Max Health doubled to: {_maxHealth}");
        }

        [ContextMenu("Test: Reset Stats")]
        private void TestResetStats()
        {
            ResetStats();
            Debug.Log("Stats reset to base values");
        }
#endif
    }
}
