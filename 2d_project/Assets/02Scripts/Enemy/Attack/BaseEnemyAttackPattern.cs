using UnityEngine;

namespace Enemy
{
    public abstract class BaseEnemyAttackPattern : ScriptableObject, IEnemyAttackPattern
    {
        [Header("공격 기본 정보")]
        [SerializeField] protected string _attackName;
        [SerializeField] protected float _attackRadius;

        public string AttackName => _attackName;
        public float AttackRadius => _attackRadius;

        public virtual bool CanExecute(EnemyEntity attacker, Transform target)
        {
            if (attacker == null || target == null)
                return false;

            float distance = Vector3.Distance(attacker.transform.position, target.position);
            return distance <= _attackRadius;
        }

        public abstract void Execute(EnemyEntity attacker, Transform target);

        public abstract void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer);
    }
}
