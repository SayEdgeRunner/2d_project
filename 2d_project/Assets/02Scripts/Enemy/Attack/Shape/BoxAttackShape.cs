using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "BoxShape", menuName = "Game/AttackShapes/Box")]
    public class BoxAttackShape : AttackShape
    {
        [Header("사각형 크기")]
        [Tooltip("전방 길이 (바라보는 방향)")]
        [SerializeField] private float _length = 3f;

        [Tooltip("좌우 너비")]
        [SerializeField] private float _width = 2f;

        public float Length => _length;
        public float Width => _width;
        public Vector2 Size => new Vector2(_length, _width);

        public override Collider2D[] GetTargetsInRange(Transform origin, Vector2 facingDirection, LayerMask targetLayer)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            float angle = GetFinalRotation(facingDirection);
            
            return Physics2D.OverlapBoxAll(center, new Vector2(_length, _width), angle, targetLayer);
        }

        public override void DrawGizmo(Transform origin, Vector2 facingDirection)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            float angle = GetFinalRotation(facingDirection);
            
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);

            Vector3 size = new Vector3(_length, _width, 0);
            
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireCube(Vector3.zero, size);
            
            Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 0.1f);
            Gizmos.DrawCube(Vector3.zero, size);

            Gizmos.matrix = oldMatrix;
        }

        public override float GetApproximateRadius()
        {
            float offsetMagnitude = Mathf.Sqrt(_forwardOffset * _forwardOffset + _verticalOffset * _verticalOffset);
            return Mathf.Sqrt(_length * _length + _width * _width) / 2f + offsetMagnitude;
        }
    }
}
