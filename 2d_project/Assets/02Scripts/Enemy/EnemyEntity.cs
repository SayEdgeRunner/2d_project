using System;
using System.Collections;
using Core;
using Pool;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMoveAIComponent), typeof(EnemyHealthComponent), typeof(EnemyDeathPresenter))]
    [RequireComponent(typeof(EnemyStatComponent))]
    public class EnemyEntity : MonoBehaviour, IPoolable, IDamageable
    {
        public event Action<EnemyEntity> OnDeathComplete;

        [Header("Debug")]
        [SerializeField] private float _deathDelay = 2f;
        [SerializeField] private Transform _targetTransform;

        private EnemyLifeState _lifeState = EnemyLifeState.Alive;
        
        private EnemyStatComponent _statComponent;
        private EnemyHealthComponent _healthComponent;
        private EnemyDeathPresenter _deathPresenter;
        private EnemyMoveAIComponent _moveAIComponent;
        private EnemyMoveComponent _moveComponent;
        private EnemyAttackHandler _attackHandler;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Collider2D[] _colliders;
        private Coroutine _deathCoroutine;
        private WaitForSeconds _deathDelayWait;

        public bool IsDead => _lifeState == EnemyLifeState.Dead;
        public EnemyLifeState LifeState => _lifeState;
        public EnemyStatComponent StatComponent => _statComponent;
        public EnemyAttackHandler AttackHandler => _attackHandler;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;

        private void Awake()
        {
            _statComponent = GetComponent<EnemyStatComponent>();
            _healthComponent = GetComponent<EnemyHealthComponent>();
            _deathPresenter = GetComponent<EnemyDeathPresenter>();
            _moveAIComponent = GetComponent<EnemyMoveAIComponent>();
            _moveComponent = GetComponent<EnemyMoveComponent>();
            _attackHandler = GetComponent<EnemyAttackHandler>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _colliders = GetComponents<Collider2D>();
            _deathDelayWait = new WaitForSeconds(_deathDelay);
        }

        private void OnEnable()
        {
            if (_healthComponent)
            {
                _healthComponent.OnDeath += HandleDeath;
            }
        }

        private void OnDisable()
        {
            if (_healthComponent)
            {
                _healthComponent.OnDeath -= HandleDeath;
            }

            if (_deathCoroutine != null)
            {
                StopCoroutine(_deathCoroutine);
                _deathCoroutine = null;
            }
        }

        public void Initialize(Transform target, EnemyConfig config, float healthMultiplier = 1f, float moveSpeedMultiplier = 1f)
        {
            _targetTransform = target;
            
            if (_statComponent)
            {
                _statComponent.Initialize(config, healthMultiplier, moveSpeedMultiplier);
            }
            
            if (_moveAIComponent && _targetTransform)
            {
                _moveAIComponent.Initialize(_targetTransform);
            }
            
            if (_healthComponent && _statComponent)
            {
                _healthComponent.SetMaxHealth(_statComponent.MaxHealth);
            }
            
            if (_moveComponent && _statComponent)
            {
                _moveComponent.SetMoveSpeed(_statComponent.MoveSpeed);
            }
        }

        public void TakeDamage(float damage)
        {
            if (_lifeState != EnemyLifeState.Alive)
                return;

            if (_healthComponent)
            {
                _healthComponent.TakeDamage(damage);
            }
        }

        private void HandleDeath()
        {
            if (_lifeState != EnemyLifeState.Alive)
                return;
            
            _lifeState = EnemyLifeState.Dying;
            
            if (_moveAIComponent)
            {
                _moveAIComponent.enabled = false;
            }
            
            foreach (var col in _colliders)
            {
                if (col) col.enabled = false;
            }
            
            if (_deathPresenter)
            {
                _deathPresenter.PlayDeathEffect();
            }
            
            _deathCoroutine = StartCoroutine(TransitionToDeadState());
        }

        private IEnumerator TransitionToDeadState()
        {
            yield return _deathDelayWait;

            _lifeState = EnemyLifeState.Dead;
            OnDeathComplete?.Invoke(this);
        }
        
        public void OnCreatedInPool()
        {
        }

        public void OnGetFromPool()
        {
            _lifeState = EnemyLifeState.Alive;
            
            if (_healthComponent)
            {
                _healthComponent.ResetHealth();
            }
            
            if (_moveAIComponent)
            {
                _moveAIComponent.enabled = true;
            }
            
            foreach (var col in _colliders)
            {
                if (col) col.enabled = true;
            }
            
            if (_deathCoroutine != null)
            {
                StopCoroutine(_deathCoroutine);
                _deathCoroutine = null;
            }
        }

        public void OnReturnToPool()
        {
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Test: Take 10 Damage")]
        private void TestDamage10()
        {
            TakeDamage(10f);
        }

        [ContextMenu("Test: Take 50 Damage")]
        private void TestDamage50()
        {
            TakeDamage(50f);
        }

        [ContextMenu("Test: Take 100 Damage (Kill)")]
        private void TestDamageKill()
        {
            TakeDamage(100f);
        }

        [ContextMenu("Test: Heal 30 HP")]
        private void TestHeal()
        {
            _healthComponent?.Heal(30f);
        }

        [ContextMenu("Test: Reset Health")]
        private void TestResetHealth()
        {
            _healthComponent?.ResetHealth();
            _lifeState = EnemyLifeState.Alive;
        }

        [ContextMenu("Test: Execute First Attack")]
        private void TestExecuteFirstAttack()
        {
            if (_statComponent == null)
            {
                Debug.LogError("[EnemyEntity] StatComponent is null!");
                return;
            }

            var attack = _statComponent.GetAttackByIndex(0);
            if (attack == null)
            {
                Debug.LogError("[EnemyEntity] No attacks configured!");
                return;
            }

            if (_targetTransform == null)
            {
                Debug.LogWarning("[EnemyEntity] No target set, using self as target for test");
                attack.Execute(this, transform);
            }
            else
            {
                attack.Execute(this, _targetTransform);
            }
        }

        [ContextMenu("Test: Execute Random Attack")]
        private void TestExecuteRandomAttack()
        {
            if (_statComponent == null)
            {
                Debug.LogError("[EnemyEntity] StatComponent is null!");
                return;
            }

            var attack = _statComponent.GetRandomAttack();
            if (attack == null)
            {
                Debug.LogError("[EnemyEntity] No attacks configured!");
                return;
            }

            if (_targetTransform == null)
            {
                Debug.LogWarning("[EnemyEntity] No target set, using self as target for test");
                attack.Execute(this, transform);
            }
            else
            {
                attack.Execute(this, _targetTransform);
            }
        }

        [ContextMenu("Test: List All Attacks")]
        private void TestListAllAttacks()
        {
            if (_statComponent == null)
            {
                Debug.LogError("[EnemyEntity] StatComponent is null!");
                return;
            }

            var attacks = _statComponent.GetAllAttacks();
            Debug.Log($"[EnemyEntity] Total attacks: {attacks?.Count ?? 0}");

            if (attacks == null) return;
            for (int i = 0; i < attacks.Count; i++)
            {
                if (attacks[i] != null)
                {
                    Debug.Log($"  [{i}] {attacks[i].GetType().Name} - Radius: {attacks[i].AttackRange}");
                }
            }
        }
        #endif
    }
}
