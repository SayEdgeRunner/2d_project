using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    [CreateAssetMenu(fileName = "CircleShape", menuName = "Game/AttackShapes/Circle")]
    public class CircleAttackShape : AttackShape
    {
        [Header("타원 크기")]
        [Tooltip("전방 반지름 (바라보는 방향)")]
        [SerializeField] private float _radiusX = 2f;

        [Tooltip("좌우 반지름 (수직 방향)")]
        [SerializeField] private float _radiusY = 2f;
        
        private readonly List<Collider2D> _tempResults = new List<Collider2D>(16);

        public float RadiusX => _radiusX;
        public float RadiusY => _radiusY;
        public bool IsCircle => Mathf.Approximately(_radiusX, _radiusY);

        public override int GetTargetsInRange(Transform origin, Vector2 facingDirection, LayerMask targetLayer, List<Collider2D> results)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            var filter = GetContactFilter(targetLayer);

            if (IsCircle)
            {
                return Physics2D.OverlapCircle(center, _radiusX, filter, results);
            }
            
            float rotation = GetFinalRotation(facingDirection);
            float maxRadius = Mathf.Max(_radiusX, _radiusY);

            _tempResults.Clear();
            int count = Physics2D.OverlapCircle(center, maxRadius, filter, _tempResults);

            for (int i = 0; i < count; i++)
            {
                var col = _tempResults[i];
                if (IsPointInEllipse(col.transform.position, center, rotation))
                {
                    results.Add(col);
                }
            }

            return results.Count;
        }

        public override void DrawGizmo(Transform origin, Vector2 facingDirection)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            float rotation = GetFinalRotation(facingDirection);

            Gizmos.color = _gizmoColor;

            if (IsCircle)
            {
                DrawWireCircle(center, _radiusX);
                Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 0.1f);
                Gizmos.DrawSphere(center, _radiusX);
            }
            else
            {
                DrawWireEllipse(center, rotation);
            }
        }

        public override float GetApproximateRadius()
        {
            float offsetMagnitude = Mathf.Sqrt(_forwardOffset * _forwardOffset + _verticalOffset * _verticalOffset);
            return Mathf.Max(_radiusX, _radiusY) + offsetMagnitude;
        }

        private bool IsPointInEllipse(Vector2 point, Vector2 center, float rotationDegrees)
        {
            Vector2 diff = point - center;
            float rad = -rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float localX = diff.x * cos - diff.y * sin;
            float localY = diff.x * sin + diff.y * cos;

            return (localX * localX) / (_radiusX * _radiusX) +
                   (localY * localY) / (_radiusY * _radiusY) <= 1f;
        }

        private Vector2 GetPointOnEllipseBoundary(Vector2 center, float localAngleDegrees, float rotationDegrees)
        {
            float rad = localAngleDegrees * Mathf.Deg2Rad;
            float x = _radiusX * Mathf.Cos(rad);
            float y = _radiusY * Mathf.Sin(rad);

            float rotRad = rotationDegrees * Mathf.Deg2Rad;
            float cosRot = Mathf.Cos(rotRad);
            float sinRot = Mathf.Sin(rotRad);

            float rotatedX = x * cosRot - y * sinRot;
            float rotatedY = x * sinRot + y * cosRot;

            return center + new Vector2(rotatedX, rotatedY);
        }

        private void DrawWireEllipse(Vector2 center, float rotationDegrees, int segments = 32)
        {
            float angleStep = 360f / segments;

            Vector2 prevPoint = GetPointOnEllipseBoundary(center, 0, rotationDegrees);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector2 newPoint = GetPointOnEllipseBoundary(center, angle, rotationDegrees);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
