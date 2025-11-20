using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMoveComponent))]
    public class EnemyMoveAIComponent : MonoBehaviour
    {
        [SerializeField] private float _stopDistance = 1.5f;

        private EnemyMoveComponent _moveComponent;
        private Transform _targetTransform;

        private void Awake()
        {
            _moveComponent = GetComponent<EnemyMoveComponent>();
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        public void Initialize(Transform target)
        {
            _targetTransform = target;
        }

        private void MoveToTarget()
        {
            if (!_targetTransform) return;

            float distance = GetDistanceToTarget();

            if (distance > _stopDistance)
            {
                Vector2 direction = GetDirectionToTarget();
                _moveComponent.Move(direction);
            }
            else
            {
                _moveComponent.Stop();
            }
        }

        private Vector2 GetDirectionToTarget()
        {
            Vector2 targetPosition = _targetTransform.position;
            Vector2 enemyPosition = transform.position;
            return (targetPosition - enemyPosition).normalized;
        }

        private float GetDistanceToTarget()
        {
            return Vector2.Distance(transform.position, _targetTransform.position);
        }
    }
}
