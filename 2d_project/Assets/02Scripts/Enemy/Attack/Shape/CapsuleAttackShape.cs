using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "CapsuleShape", menuName = "Game/AttackShapes/Capsule")]
    public class CapsuleAttackShape : AttackShape
    {
        [Header("캡슐 크기")]
        [Tooltip("캡슐 반지름 (좌우 두께)")]
        [SerializeField] private float _radius = 1f;

        [Tooltip("캡슐 전체 길이 (전방 방향)")]
        [SerializeField] private float _length = 3f;

        public float Radius => _radius;
        public float Length => _length;

        public override Collider2D[] GetTargetsInRange(Transform origin, Vector2 facingDirection, LayerMask targetLayer)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            Vector2 size = new Vector2(_length, _radius * 2f);
            float angle = GetFinalRotation(facingDirection);

            return Physics2D.OverlapCapsuleAll(center, size, CapsuleDirection2D.Horizontal, angle, targetLayer);
        }

        public override void DrawGizmo(Transform origin, Vector2 facingDirection)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            float angle = GetFinalRotation(facingDirection);

            Gizmos.color = _gizmoColor;
            
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);

            float halfLength = Mathf.Max(0, _length / 2f - _radius);
            
            DrawWireCircleLocal(new Vector2(-halfLength, 0), _radius);
            DrawWireCircleLocal(new Vector2(halfLength, 0), _radius);
            
            Gizmos.DrawLine(new Vector3(-halfLength, _radius), new Vector3(halfLength, _radius));
            Gizmos.DrawLine(new Vector3(-halfLength, -_radius), new Vector3(halfLength, -_radius));

            Gizmos.matrix = oldMatrix;
        }

        public override float GetApproximateRadius()
        {
            float offsetMagnitude = Mathf.Sqrt(_forwardOffset * _forwardOffset + _verticalOffset * _verticalOffset);
            return Mathf.Max(_radius, _length / 2f) + offsetMagnitude;
        }
        
        private void DrawWireCircleLocal(Vector2 center, float radius, int segments = 32)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = new Vector3(center.x + radius, center.y, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = new Vector3(
                    center.x + Mathf.Cos(angle) * radius,
                    center.y + Mathf.Sin(angle) * radius,
                    0
                );
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
