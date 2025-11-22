using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "SectorShape", menuName = "Game/AttackShapes/Sector")]
    public class SectorAttackShape : AttackShape
    {
        [Header("부채꼴 크기")]
        [Tooltip("전방 반지름 (바라보는 방향)")]
        [SerializeField] private float _radiusX = 3f;

        [Tooltip("좌우 반지름 (수직 방향)")]
        [SerializeField] private float _radiusY = 3f;

        [Header("부채꼴 각도")]
        [Range(1f, 360f)]
        [SerializeField] private float _angle = 90f;
        
        private readonly List<Collider2D> _tempResults = new List<Collider2D>(16);

        public float RadiusX => _radiusX;
        public float RadiusY => _radiusY;
        public float Angle => _angle;
        public bool IsCircular => Mathf.Approximately(_radiusX, _radiusY);

        public override int GetTargetsInRange(Transform origin, Vector2 facingDirection, LayerMask targetLayer, List<Collider2D> results)
        {
            Vector2 center = (Vector2)origin.position + GetOffsetPosition(facingDirection);
            float rotation = GetFinalRotation(facingDirection);
            var filter = GetContactFilter(targetLayer);

            float maxRadius = Mathf.Max(_radiusX, _radiusY);

            _tempResults.Clear();
            int count = Physics2D.OverlapCircle(center, maxRadius, filter, _tempResults);

            for (int i = 0; i < count; i++)
            {
                var col = _tempResults[i];
                Vector2 targetPos = col.transform.position;

                if (IsPointInSector(targetPos, center, rotation))
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
            float halfAngle = _angle / 2f;

            Gizmos.color = _gizmoColor;
            
            Vector2 startPoint = GetPointOnEllipseBoundary(center, -halfAngle, rotation);
            Vector2 endPoint = GetPointOnEllipseBoundary(center, halfAngle, rotation);
            
            Gizmos.DrawLine(center, startPoint);
            Gizmos.DrawLine(center, endPoint);
            
            DrawEllipseArc(center, rotation, _angle);
            
            Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, 0.1f);
            DrawFilledEllipseSector(center, rotation, _angle);
        }

        public override float GetApproximateRadius()
        {
            float offsetMagnitude = Mathf.Sqrt(_forwardOffset * _forwardOffset + _verticalOffset * _verticalOffset);
            return Mathf.Max(_radiusX, _radiusY) + offsetMagnitude;
        }

        private bool IsPointInSector(Vector2 point, Vector2 center, float rotationDegrees)
        {
            Vector2 toPoint = point - center;
            float localAngle = GetLocalAngle(toPoint, rotationDegrees);

            float halfAngle = _angle / 2f;
            if (Mathf.Abs(localAngle) > halfAngle)
                return false;

            return IsInsideEllipse(toPoint, rotationDegrees);
        }
        
        private float GetLocalAngle(Vector2 direction, float rotationDegrees)
        {
            float rad = -rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float localX = direction.x * cos - direction.y * sin;
            float localY = direction.x * sin + direction.y * cos;

            return Mathf.Atan2(localY, localX) * Mathf.Rad2Deg;
        }
        
        private bool IsInsideEllipse(Vector2 localDirection, float rotationDegrees)
        {
            float rad = -rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float localX = localDirection.x * cos - localDirection.y * sin;
            float localY = localDirection.x * sin + localDirection.y * cos;

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

        private void DrawEllipseArc(Vector2 center, float rotationDegrees, float arcAngle, int segments = 32)
        {
            float halfArc = arcAngle / 2f;
            float angleStep = arcAngle / segments;

            Vector2 prevPoint = GetPointOnEllipseBoundary(center, -halfArc, rotationDegrees);

            for (int i = 1; i <= segments; i++)
            {
                float angle = -halfArc + (i * angleStep);
                Vector2 newPoint = GetPointOnEllipseBoundary(center, angle, rotationDegrees);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }

        private void DrawFilledEllipseSector(Vector2 center, float rotationDegrees, float arcAngle, int segments = 16)
        {
            float halfArc = arcAngle / 2f;
            float angleStep = arcAngle / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = -halfArc + (i * angleStep);
                float angle2 = -halfArc + ((i + 1) * angleStep);

                Vector2 p1 = GetPointOnEllipseBoundary(center, angle1, rotationDegrees);
                Vector2 p2 = GetPointOnEllipseBoundary(center, angle2, rotationDegrees);

                Gizmos.DrawLine(center, p1);
                Gizmos.DrawLine(center, p2);
            }
        }
    }
}
