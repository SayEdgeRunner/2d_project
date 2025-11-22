using UnityEngine;

namespace Enemy
{
    public interface IEnemyAttackPattern
    {
        string AttackName { get; }
        float AttackRange { get; }
        bool CanExecute(EnemyEntity attacker, Transform target);
        void Execute(EnemyEntity attacker, Transform target);
        void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer);
    }
}
