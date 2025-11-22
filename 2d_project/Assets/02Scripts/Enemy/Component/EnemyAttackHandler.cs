using UnityEngine;

namespace Enemy
{
    public class EnemyAttackHandler : MonoBehaviour
    {
        [Header("공격 판정")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _targetLayer;

        private BaseEnemyAttackPattern _currentAttack;
        private Transform _currentTarget;
        private EnemyEntity _entity;

        private void Awake()
        {
            _entity = GetComponent<EnemyEntity>();
        }

        public void PrepareAttack(BaseEnemyAttackPattern attack, Transform target)
        {
            _currentAttack = attack;
            _currentTarget = target;
        }

        public void OnAttackHit()
        {
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
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Transform origin = _attackPoint != null ? _attackPoint : transform;
            Vector2 facingDirection = GetFacingDirection();

            // 현재 준비된 공격의 범위 표시
            if (_currentAttack != null)
            {
                // MeleeAttackPattern인 경우 Shape 사용
                if (_currentAttack is MeleeAttackPattern meleeAttack && meleeAttack.AttackShape != null)
                {
                    meleeAttack.AttackShape.DrawGizmo(origin, facingDirection);
                }
                else
                {
                    // Shape가 없거나 다른 패턴일 경우 기본 원형 표시
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(origin.position, _currentAttack.AttackRange);

                    Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
                    Gizmos.DrawSphere(origin.position, _currentAttack.AttackRange);
                }
            }
            else
            {
                // 공격이 없을 때 기본 표시 (노란색)
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(origin.position, 0.5f);
            }
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosSelected();

            // 항상 공격 포인트 위치 표시
            if (_attackPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_attackPoint.position, 0.2f);
            }

            // 바라보는 방향 표시
            Vector2 facingDirection = GetFacingDirection();
            Gizmos.color = Color.green;
            Vector3 start = _attackPoint != null ? _attackPoint.position : transform.position;
            Gizmos.DrawLine(start, start + (Vector3)(facingDirection * 0.5f));
        }

        private Vector2 GetFacingDirection()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            return Vector2.right;
        }
#endif
    }
}
