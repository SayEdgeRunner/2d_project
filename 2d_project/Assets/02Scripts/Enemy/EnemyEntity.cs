using System;
using System.Collections;
using Core;
using Pool;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMoveAIComponent), typeof(EnemyHealthComponent), typeof(EnemyDeathPresenter))]
    public class EnemyEntity : MonoBehaviour, IPoolable, IDamageable
    {
        public event Action<EnemyEntity> OnDeathComplete;

        [Header("Debug")]
        [SerializeField] private EnemyLifeState _lifeState = EnemyLifeState.Alive;
        [SerializeField] private float _deathDelay = 2f;

        private EnemyHealthComponent _healthComponent;
        private EnemyDeathPresenter _deathPresenter;
        private EnemyMoveAIComponent _moveAIComponent;
        private EnemyMoveComponent _moveComponent;
        private Collider2D[] _colliders;
        private Transform _targetTransform;
        private Coroutine _deathCoroutine;

        public bool IsDead => _lifeState == EnemyLifeState.Dead;
        public EnemyLifeState LifeState => _lifeState;

        private void Awake()
        {
            _healthComponent = GetComponent<EnemyHealthComponent>();
            _deathPresenter = GetComponent<EnemyDeathPresenter>();
            _moveAIComponent = GetComponent<EnemyMoveAIComponent>();
            _moveComponent = GetComponent<EnemyMoveComponent>();
            _colliders = GetComponents<Collider2D>();
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
            if (_moveAIComponent && _targetTransform)
            {
                _moveAIComponent.Initialize(_targetTransform);
            }
            
            if (_healthComponent)
            {
                float finalHealth = config.GetFinalHealth(healthMultiplier);
                _healthComponent.SetMaxHealth(finalHealth);
            }
            
            if (_moveComponent)
            {
                float finalSpeed = config.GetFinalMoveSpeed(moveSpeedMultiplier);
                _moveComponent.SetMoveSpeed(finalSpeed);
            }
        }

        public void TakeDamage(float damage)
        {
            // Dying 또는 Dead 상태에서는 데미지 무시
            if (_lifeState != EnemyLifeState.Alive)
                return;

            if (_healthComponent)
            {
                _healthComponent.TakeDamage(damage);
            }
        }

        private void HandleDeath()
        {
            // 중복 호출 방지
            if (_lifeState != EnemyLifeState.Alive)
                return;

            // Life State를 Dying으로 변경
            _lifeState = EnemyLifeState.Dying;

            // 1. AI 비활성화
            if (_moveAIComponent)
            {
                _moveAIComponent.enabled = false;
            }

            // 2. Collider 비활성화 (추가 충돌 방지)
            foreach (var col in _colliders)
            {
                if (col) col.enabled = false;
            }

            // 3. 사망 연출 재생
            if (_deathPresenter)
            {
                _deathPresenter.PlayDeathEffect();
            }

            // 4. 지연 후 Dead 상태로 전환
            _deathCoroutine = StartCoroutine(TransitionToDeadState());
        }

        private IEnumerator TransitionToDeadState()
        {
            yield return new WaitForSeconds(_deathDelay);

            _lifeState = EnemyLifeState.Dead;
            OnDeathComplete?.Invoke(this);
        }

        // IPoolable 구현
        public void OnCreatedInPool()
        {
            // 컴포넌트 참조는 Awake에서 처리
        }

        public void OnGetFromPool()
        {
            // Life State 리셋
            _lifeState = EnemyLifeState.Alive;

            // 체력 리셋
            if (_healthComponent)
            {
                _healthComponent.ResetHealth();
            }

            // AI 활성화
            if (_moveAIComponent)
            {
                _moveAIComponent.enabled = true;
            }

            // Collider 활성화
            foreach (var col in _colliders)
            {
                if (col) col.enabled = true;
            }

            // 코루틴 정리
            if (_deathCoroutine != null)
            {
                StopCoroutine(_deathCoroutine);
                _deathCoroutine = null;
            }
        }

        public void OnReturnToPool()
        {
            // 정리 작업
        }

        // ===== 테스트용 메서드 =====
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
        #endif
    }
}
