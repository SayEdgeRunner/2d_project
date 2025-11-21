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

            Vector2 directionVector = (Vector2)_targetTransform.position - (Vector2)transform.position;

            if (directionVector.sqrMagnitude > _stopDistance * _stopDistance)
            {
                _moveComponent.Move(directionVector.normalized);
            }
            else
            {
                _moveComponent.Stop();
            }
        }
    }
}
