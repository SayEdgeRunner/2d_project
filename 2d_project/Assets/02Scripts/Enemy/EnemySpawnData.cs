using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemySpawnData
    {
        [SerializeField] private EnemyEntity enemyPrefab;
        [SerializeField] private int weight = 1;

        public EnemyEntity EnemyPrefab => enemyPrefab;
        public int Weight => weight;

        public EnemySpawnData(EnemyEntity enemyPrefab, int weight)
        {
            this.enemyPrefab = enemyPrefab;
            this.weight = Mathf.Max(1, weight);
        }
    }
}
