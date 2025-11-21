using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemySpawnData
    {
        [SerializeField] private EnemyEntity _enemyPrefab;
        [SerializeField] private int _weight;

        public EnemyEntity EnemyPrefab => _enemyPrefab;
        public int Weight => _weight;

        public EnemySpawnData(EnemyEntity enemyPrefab, int weight)
        {
            _enemyPrefab = enemyPrefab;
            _weight = Mathf.Max(1, weight);
        }
    }
}
