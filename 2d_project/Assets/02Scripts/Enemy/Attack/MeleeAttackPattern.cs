using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "MeleeAttack", menuName = "Game/Attacks/Melee")]
    public class MeleeAttackPattern : BaseEnemyAttackPattern
    {
        [Header("근접 공격 설정")]
        [SerializeField] private float _damage = 10f;

        [Header("판정 형태")]
        [Tooltip("공격 판정 영역 (Circle, Sector, Box, Capsule 등)")]
        [SerializeField] private AttackShape _attackShape;

        [Header("애니메이션")]
        [Tooltip("재생할 공격 애니메이션 트리거 이름")]
        [SerializeField] private string _attackAnimationTrigger = "Attack";

        [Header("텔레그래프 설정")]
        [Tooltip("공격 예고 표시 시간 (0이면 표시 안함)")]
        [SerializeField] private float _telegraphDuration = 0f;

        [Tooltip("텔레그래프 채우기 색상")]
        [SerializeField] private Color _telegraphColor = new Color(1f, 0f, 0f, 0.3f);

        public float Damage => _damage;
        public string AttackAnimationTrigger => _attackAnimationTrigger;
        public AttackShape AttackShape => _attackShape;
        public float TelegraphDuration => _telegraphDuration;
        public Color TelegraphColor => _telegraphColor;

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
            if (_attackShape == null)
            {
                Debug.LogError($"[{_attackName}] AttackShape is not assigned!");
                return;
            }

            Transform origin = attackPoint ? attackPoint : attacker.transform;
            Vector2 facingDirection = GetFacingDirection(attacker);
            
            Collider2D[] hits = _attackShape.GetTargetsInRange(origin, facingDirection, targetLayer);

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
