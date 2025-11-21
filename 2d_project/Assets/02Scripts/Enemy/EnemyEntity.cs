using System;
using Pool;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMoveAIComponent))]
    public class EnemyEntity : MonoBehaviour, IPoolable
    {
        public event Action<EnemyEntity> OnDeath;

        private EnemyMoveAIComponent _moveAIComponent;
        private Transform _targetTransform;

        private void Awake()
        {
            _moveAIComponent = GetComponent<EnemyMoveAIComponent>();
        }

        public void Initialize(Transform target)
        {
            _targetTransform = target;
            if (_moveAIComponent && _targetTransform)
            {
                _moveAIComponent.Initialize(_targetTransform);
            }
        }

        public void OnCreatedInPool()
        {
        }

        public void OnGetFromPool()
        {
        }

        public void OnReturnToPool()
        {
        }

        public void Die()
        {
            OnDeath?.Invoke(this);
        }
    }
}
