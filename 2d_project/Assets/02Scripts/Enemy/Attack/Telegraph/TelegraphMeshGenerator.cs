using UnityEngine;

namespace Enemy
{
    public static class TelegraphMeshGenerator
    {
        public static Mesh CreateQuadMesh(AttackShape shape, Vector2 facingDirection)
        {
            float size = GetMeshSize(shape);
            return CreateQuad(size, size);
        }
        
        public static Mesh CreateQuad(float width, float height)
        {
            Mesh mesh = new Mesh();
            mesh.name = "TelegraphQuad";

            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-halfWidth, -halfHeight, 0),
                new Vector3(halfWidth, -halfHeight, 0),
                new Vector3(-halfWidth, halfHeight, 0),
                new Vector3(halfWidth, halfHeight, 0)
            };

            int[] triangles = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };

            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private static float GetMeshSize(AttackShape shape)
        {
            return shape.GetApproximateRadius() * 2f;
        }
        
        public static Vector2 GetCircleSize(CircleAttackShape circle)
        {
            return new Vector2(circle.RadiusX * 2f, circle.RadiusY * 2f);
        }
        
        public static Vector2 GetSectorSize(SectorAttackShape sector)
        {
            return new Vector2(sector.RadiusX * 2f, sector.RadiusY * 2f);
        }
        
        public static Vector2 GetBoxSize(BoxAttackShape box)
        {
            return new Vector2(box.Length, box.Width);
        }
        
        public static Vector2 GetCapsuleSize(CapsuleAttackShape capsule)
        {
            return new Vector2(capsule.Length, capsule.Radius * 2f);
        }
    }
}
