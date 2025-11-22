using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public abstract class AttackShape : ScriptableObject
    {
        protected ContactFilter2D _contactFilter;
        protected bool _contactFilterInitialized;
        protected LayerMask _cachedLayerMask;

        [Header("위치 설정")]
        [Tooltip("전방 오프셋 (바라보는 방향으로 이동)")]
        [SerializeField] protected float _forwardOffset = 0f;

        [Tooltip("상하 오프셋 (양수: 위, 음수: 아래)")]
        [SerializeField] protected float _verticalOffset = 0f;

        [Tooltip("체크 시 바라보는 방향에 따라 오프셋이 회전함")]
        [SerializeField] protected bool _followFacingDirection = true;

        [Header("회전 설정")]
        [Tooltip("추가 회전 각도 (바라보는 방향 기준)")]
        [Range(-180f, 180f)]
        [SerializeField] protected float _rotationOffset = 0f;

        [Header("디버그")]
        [SerializeField] protected Color _gizmoColor = Color.red;

        public float ForwardOffset => _forwardOffset;
        public float VerticalOffset => _verticalOffset;
        public bool FollowFacingDirection => _followFacingDirection;
        public float RotationOffset => _rotationOffset;
        public Color GizmoColor => _gizmoColor;
        
        public abstract int GetTargetsInRange(
            Transform origin,
            Vector2 facingDirection,
            LayerMask targetLayer,
            List<Collider2D> results
        );

        public abstract void DrawGizmo(Transform origin, Vector2 facingDirection);

        public abstract float GetApproximateRadius();

        protected ContactFilter2D GetContactFilter(LayerMask targetLayer)
        {
            if (!_contactFilterInitialized || _cachedLayerMask != targetLayer)
            {
                _contactFilter = new ContactFilter2D();
                _contactFilter.SetLayerMask(targetLayer);
                _contactFilter.useTriggers = true;
                _cachedLayerMask = targetLayer;
                _contactFilterInitialized = true;
            }
            return _contactFilter;
        }
        
        protected Vector2 GetOffsetPosition(Vector2 facingDirection)
        {
            if (!_followFacingDirection)
            {
                return new Vector2(_forwardOffset, _verticalOffset);
            }
            
            Vector2 forward = facingDirection * _forwardOffset;
            Vector2 perpendicular = new Vector2(-facingDirection.y, facingDirection.x) * _verticalOffset;

            return forward + perpendicular;
        }
        
        protected float GetFinalRotation(Vector2 facingDirection)
        {
            float baseAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            return baseAngle + _rotationOffset;
        }
        
        protected Vector2 GetFinalDirection(Vector2 facingDirection)
        {
            return RotateVector(facingDirection, _rotationOffset);
        }
        
        protected Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }
        
        protected void DrawWireCircle(Vector2 center, float radius, int segments = 32)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector2(radius, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
        
        protected void DrawArc(Vector2 center, float radius, Vector2 direction, float angle, int segments = 32)
        {
            float halfAngle = angle / 2f;
            float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - halfAngle;
            float angleStep = angle / segments;

            Vector3 prevPoint = center + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right * radius);

            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = startAngle + (i * angleStep);
                Vector3 newPoint = center + (Vector2)(Quaternion.Euler(0, 0, currentAngle) * Vector2.right * radius);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
