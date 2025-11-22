using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 근접 공격 패턴
    /// 데미지 판정은 애니메이션 이벤트에서 OnAttackHit() 호출 시 발생합니다.
    /// 애니메이션 에디터에서 원하는 프레임에 "OnAttackHit" 이벤트를 추가하세요.
    /// </summary>
    [CreateAssetMenu(fileName = "MeleeAttack", menuName = "Game/Attacks/Melee")]
    public class MeleeAttackPattern : BaseEnemyAttackPattern
    {
        [Header("근접 공격 설정")]
        [SerializeField] private float _damage = 10f;

        [Header("애니메이션")]
        [Tooltip("재생할 공격 애니메이션 트리거 이름")]
        [SerializeField] private string _attackAnimationTrigger = "Attack";

        public float Damage => _damage;
        public string AttackAnimationTrigger => _attackAnimationTrigger;

        public override void Execute(EnemyEntity attacker, Transform target)
        {
            if (attacker == null)
            {
                Debug.LogWarning($"[{_attackName}] Attacker is null!");
                return;
            }

            var animator = attacker.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"[{_attackName}] Attacker has no Animator component!");
                return;
            }

            var attackHandler = attacker.GetComponent<EnemyAttackHandler>();
            if (attackHandler == null)
            {
                Debug.LogError($"[{_attackName}] Attacker has no EnemyAttackHandler component!");
                return;
            }

            attackHandler.PrepareAttack(this, target);

            if (!string.IsNullOrEmpty(_attackAnimationTrigger))
            {
                animator.SetTrigger(_attackAnimationTrigger);
            }
            else
            {
                Debug.LogWarning($"[{_attackName}] No animation trigger set, attack will not play!");
            }
        }

        public override void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer)
        {
            Transform attackOrigin = attackPoint ? attackPoint : attacker.transform;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackOrigin.position,
                _attackRadius,
                targetLayer
            );

            HashSet<GameObject> hitObjects = new HashSet<GameObject>();

            foreach (var hit in hits)
            {
                if (hit.gameObject == attacker.gameObject) continue;
                if (hitObjects.Contains(hit.gameObject)) continue;

                var damageable = hit.GetComponent<IDamageable>();
                if (damageable == null) continue;
                
                damageable.TakeDamage(_damage);
                hitObjects.Add(hit.gameObject);
            }
        }
    }
}
