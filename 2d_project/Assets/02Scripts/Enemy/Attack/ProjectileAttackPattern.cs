using Core;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "ProjectileAttack", menuName = "Game/Attacks/Projectile")]
    public class ProjectileAttackPattern : BaseEnemyAttackPattern
    {
        [Header("투사체 설정")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;

        public override void Execute(EnemyEntity attacker, Transform target)
        {
            if (attacker == null || target == null)
            {
                Debug.LogWarning($"[{AttackName}] Attacker or target is null!");
                return;
            }

            if (_projectilePrefab == null)
            {
                Debug.LogError($"[{AttackName}] Projectile prefab is not assigned!");
                return;
            }

            var projectile = PrefabPoolManager.Get(_projectilePrefab);
            if (projectile == null)
            {
                Debug.LogError($"[{AttackName}] Failed to get projectile from pool!");
                return;
            }

            projectile.transform.position = attacker.transform.position;

            Vector3 direction = (target.position - attacker.transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            projectile.SetActive(true);
        }

        public override void OnHit(EnemyEntity attacker, Transform target, Transform attackPoint, LayerMask targetLayer)
        {
        }
    }
}
