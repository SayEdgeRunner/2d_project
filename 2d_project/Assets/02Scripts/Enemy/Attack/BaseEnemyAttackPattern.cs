using UnityEngine;

namespace Enemy
{
    public abstract class BaseEnemyAttackPattern : ScriptableObject, IEnemyAttackPattern
    {
        [Header("공격 기본 정보")]
        [SerializeField] protected string _attackName;

        [Tooltip("공격 가능 거리 (타겟과의 거리가 이 값 이하일 때 공격 가능)")]
        [SerializeField] protected float _attackRange = 2f;

        public string AttackName => _attackName;
        public float AttackRange => _attackRange;

        public virtual bool CanExecute(EnemyEntity attacker, Transform target)
        {
            if (attacker == null || target == null)
                return false;

            float distance = Vector3.Distance(attacker.transform.position, target.position);
            return distance <= _attackRange;
        }

        public abstract void Execute(EnemyEntity attacker, Transform target);

        public abstract void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer);

        protected Vector2 GetFacingDirection(EnemyEntity attacker)
        {
            var spriteRenderer = attacker.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            return Vector2.right;
        }
    }
}
