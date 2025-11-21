using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class PoolConfiguration
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialCount = 10;

        public GameObject Prefab => _prefab;
        public int InitialCount => _initialCount;
    }
}