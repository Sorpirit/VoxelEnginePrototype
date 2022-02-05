using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MeshData
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<int> Triangles = new List<int>();
        public readonly List<Vector2> UV = new List<Vector2>();

        public readonly List<Vector3> ColliderVertices = new List<Vector3>();
        public readonly List<int> ColliderTriangles = new List<int>();
        
        public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
        {
            Vertices.Add(vertex);
            if (vertexGeneratesCollider)
            {
                ColliderVertices.Add(vertex);
            }
        }

        public void AddQuadTriangles(bool quadGeneratesCollider)
        {
            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 2);

            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 1);

            if (quadGeneratesCollider)
            {
                ColliderTriangles.Add(ColliderVertices.Count - 4);
                ColliderTriangles.Add(ColliderVertices.Count - 3);
                ColliderTriangles.Add(ColliderVertices.Count - 2);
                ColliderTriangles.Add(ColliderVertices.Count - 4);
                ColliderTriangles.Add(ColliderVertices.Count - 2);
                ColliderTriangles.Add(ColliderVertices.Count - 1);
            }
        }
    }
}