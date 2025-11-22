using Core;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackHandler : MonoBehaviour, ITimeScalable
    {
        [Header("공격 판정")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _targetLayer;

        [Header("텔레그래프")]
        [SerializeField] private AttackTelegraph _telegraph;

        [Header("컴포넌트 참조")]
        [SerializeField] private Animator _animator;

        private BaseEnemyAttackPattern _currentAttack;
        private Transform _currentTarget;
        private EnemyEntity _entity;
        private SpriteRenderer _spriteRenderer;
        private float _timeScale = 1f;

        private void Awake()
        {
            _entity = GetComponent<EnemyEntity>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            EnemyTimeManager.Instance?.Register(this);
        }

        private void OnDisable()
        {
            EnemyTimeManager.Instance?.Unregister(this);
        }

        public void SetTimeScale(float scale)
        {
            _timeScale = scale;

            if (_animator != null)
            {
                _animator.speed = scale;
            }
        }

        public void Pause() => SetTimeScale(0f);
        public void Resume() => SetTimeScale(1f);

        public void PrepareAttack(BaseEnemyAttackPattern attack, Transform target)
        {
            _currentAttack = attack;
            _currentTarget = target;

            if (_telegraph == null
                || attack is not MeleeAttackPattern meleeAttack
                || meleeAttack.TelegraphDuration <= 0
                || meleeAttack.AttackShape == null) return;
            
            Vector2 facingDirection = GetFacingDirection();
            _telegraph.Show(
                meleeAttack.AttackShape,
                meleeAttack.TelegraphDuration,
                meleeAttack.TelegraphColor,
                facingDirection
            );
        }

        public void OnAttackHit()
        {
            _telegraph?.Hide();

            if (_currentAttack == null)
            {
                Debug.LogWarning("[EnemyAttackHandler] No attack prepared!");
                return;
            }

            _currentAttack.OnHit(_entity, _currentTarget, _attackPoint, _targetLayer);
        }

        public void ClearAttack()
        {
            _currentAttack = null;
            _currentTarget = null;
            _telegraph?.Hide();
        }

        private Vector2 GetFacingDirection()
        {
            if (_spriteRenderer != null)
            {
                return _spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            return Vector2.right;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Transform origin = _attackPoint != null ? _attackPoint : transform;
            Vector2 facingDirection = GetFacingDirection();

            if (_currentAttack != null)
            {
                if (_currentAttack is MeleeAttackPattern meleeAttack && meleeAttack.AttackShape != null)
                {
                    meleeAttack.AttackShape.DrawGizmo(origin, facingDirection);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(origin.position, _currentAttack.AttackRange);

                    Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
                    Gizmos.DrawSphere(origin.position, _currentAttack.AttackRange);
                }
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(origin.position, 0.5f);
            }
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosSelected();

            if (_attackPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_attackPoint.position, 0.2f);
            }

            Vector2 facingDirection = GetFacingDirection();
            Gizmos.color = Color.green;
            Vector3 start = _attackPoint != null ? _attackPoint.position : transform.position;
            Gizmos.DrawLine(start, start + (Vector3)(facingDirection * 0.5f));
        }
#endif
    }
}
