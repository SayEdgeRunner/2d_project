using Redcode.Pools;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMoveAIComponent))]
    public class EnemyEntity : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Transform _targetTransform;

        private EnemyMoveAIComponent _moveAIComponent;

        private void Awake()
        {
            _moveAIComponent = GetComponent<EnemyMoveAIComponent>();
        }

        private void Start()
        {
            if (_targetTransform != null)
            {
                _moveAIComponent.Initialize(_targetTransform);
            }
        }

        public void OnCreatedInPool()
        {
        }

        public void OnGettingFromPool()
        {
        }
    }
}
