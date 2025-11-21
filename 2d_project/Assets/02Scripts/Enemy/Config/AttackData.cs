using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public struct AttackData
    {
        [Tooltip("공격 이름 (예: '근접 베기', '수리검')")]
        public string AttackName;

        [Tooltip("공격 타입")]
        public EAttackType Type;

        [Tooltip("공격 데미지")]
        public float Damage;

        [Tooltip("공격 범위")]
        public float AttackRadius;
    }
}