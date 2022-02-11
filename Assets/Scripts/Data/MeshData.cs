using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public struct MeshData
    {
        public static MeshData Default => new MeshData(50);
        
        public readonly List<Vector3> Vertices;
        public readonly List<int> Triangles;
        public readonly List<Vector2> UV;

        public readonly bool IsValid;

        public MeshData(int verticesAmount = 100)
        {
            Vertices = new List<Vector3>(verticesAmount);
            Triangles = new List<int>(verticesAmount);
            UV = new List<Vector2>();
            IsValid = true;
        }

        public readonly void AddVertex(Vector3 vertex)
        {
            Vertices.Add(vertex);
        }

        public readonly void AddQuadTriangles()
        {
            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 2);
            
            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 1);
        }

        public readonly void Clear()
        {
            Vertices.Clear();
            Triangles.Clear();
            UV.Clear();
        }
    }
}