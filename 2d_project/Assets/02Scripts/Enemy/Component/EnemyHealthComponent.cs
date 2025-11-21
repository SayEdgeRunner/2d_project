using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealthComponent : MonoBehaviour
    {
        [Header("체력 설정")]
        [SerializeField] private float _maxHealth = 100f;

        private float _currentHealth;
        private bool _isDead;

        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDeath;

        public bool IsDead => _isDead;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _isDead = false;
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0) return;

            _currentHealth = Mathf.Max(_currentHealth - damage, 0f);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (_isDead || amount <= 0) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void SetMaxHealth(float maxHealth)
        {
            _maxHealth = Mathf.Max(1f, maxHealth);
            _currentHealth = _maxHealth;
            _isDead = false;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            _isDead = false;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public float GetHealthPercentage()
        {
            return _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;
        }

        public bool IsAlive()
        {
            return !_isDead && _currentHealth > 0f;
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            OnDeath?.Invoke();
        }
    }
}
