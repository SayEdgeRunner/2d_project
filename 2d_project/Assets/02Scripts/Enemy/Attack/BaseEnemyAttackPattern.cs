using UnityEngine;

namespace Enemy
{
    public abstract class BaseEnemyAttackPattern : ScriptableObject, IEnemyAttackPattern
    {
        [Header("공격 기본 정보")]
        [SerializeField] protected string InternalAttackName;

        [Tooltip("공격 가능 거리 (타겟과의 거리가 이 값 이하일 때 공격 가능)")]
        [SerializeField] protected float InternalAttackRange = 2f;

        public string AttackName => InternalAttackName;
        public float AttackRange => InternalAttackRange;

        public virtual bool CanExecute(EnemyEntity attacker, Transform target)
        {
            if (attacker == null || target == null)
                return false;

            float sqrDistance = (attacker.transform.position - target.position).sqrMagnitude;
            return sqrDistance <= InternalAttackRange * InternalAttackRange;
        }

        public abstract void Execute(EnemyEntity attacker, Transform target);

        public abstract void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer);

        protected Vector2 GetFacingDirection(EnemyEntity attacker)
        {
            var spriteRenderer = attacker.SpriteRenderer;
            if (spriteRenderer != null)
            {
                return spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            return Vector2.right;
        }
    }
}
