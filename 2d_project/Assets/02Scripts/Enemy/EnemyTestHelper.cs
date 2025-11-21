using UnityEngine;

namespace Enemy
{
    public class EnemyTestHelper : MonoBehaviour
    {
        private EnemyEntity _enemyEntity;

        private void Awake()
        {
            _enemyEntity = GetComponent<EnemyEntity>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (_enemyEntity != null)
                {
                    Debug.Log($"[EnemyTestHelper] Killing enemy manually with K key");
                    _enemyEntity.Die();
                }
            }
        }
    }
}
