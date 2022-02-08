using Data;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    
    public bool ShowGizmo = false;

    public ChunkData ChunkData { get; private set; }

    public bool ModifiedByThePlayer
    {
        get
        {
            return ChunkData.ModifiedByThePlayer;
        }
        set
        {
            ChunkData.ModifiedByThePlayer = value;
        }
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshCollider.sharedMesh = new Mesh();
        _mesh = _meshFilter.mesh;
    }

    public void InitializeChunk(ChunkData data)
    {
        ChunkData = data;
    }

    private void RenderMesh(MeshData meshData)
    {
        _mesh.Clear();

        var vert = meshData.Vertices.ToArray();
        var tris = meshData.Triangles.ToArray();

        _mesh.subMeshCount = 1;
        _mesh.vertices = vert;

        _mesh.SetTriangles(tris, 0);

        _mesh.uv = meshData.UV.ToArray();
        _mesh.RecalculateNormals();

        var sharedMesh = _meshCollider.sharedMesh;
        sharedMesh.Clear();
        sharedMesh.SetVertices(vert);
        sharedMesh.SetTriangles(tris, 0);
        _meshCollider.sharedMesh = sharedMesh;
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (ShowGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.ChunkSize / 2f, ChunkData.ChunkHeight / 2f, ChunkData.ChunkSize / 2f), new Vector3(ChunkData.ChunkSize, ChunkData.ChunkHeight, ChunkData.ChunkSize));
            }
        }
    }
#endif
}