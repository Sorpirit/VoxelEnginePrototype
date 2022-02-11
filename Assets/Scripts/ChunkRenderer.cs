using Data;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    public bool ShowGizmo = false;

    public ChunkData ChunkData { 
        get => _chunkData;
        set
        {
            _chunkData = value;
            UpdateChunk();
        }
    }
    
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    private MeshData _data;
    private ChunkData _chunkData;
    
    public bool ModifiedByThePlayer
    {
        get
        {
            return _chunkData.ModifiedByThePlayer;
        }
        set
        {
            _chunkData.ModifiedByThePlayer = value;
        }
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshCollider.sharedMesh = new Mesh();
        _mesh = _meshFilter.mesh;
        
        _data = MeshData.Default;
    }

    private void RenderMesh()
    {
        _mesh.Clear();

        var vert = _data.Vertices.ToArray();
        var tris = _data.Triangles.ToArray();

        _mesh.subMeshCount = 1;
        _mesh.vertices = vert;

        _mesh.SetTriangles(tris, 0);

        _mesh.uv = _data.UV.ToArray();
        _mesh.RecalculateNormals();

        var sharedMesh = _meshCollider.sharedMesh;
        sharedMesh.Clear();
        sharedMesh.SetVertices(vert);
        sharedMesh.SetTriangles(tris, 0);
        _meshCollider.sharedMesh = sharedMesh;
    }

    public void UpdateChunk()
    {
        _data.Clear();
        Chunk.LoadMeshData(ChunkData, _data);
        RenderMesh();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (ShowGizmo)
        {
            if (Application.isPlaying && ChunkData.IsValid)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(World.Instance.ChunkSize / 2f, World.Instance.ChunkHeight / 2f, World.Instance.ChunkSize / 2f), new Vector3(World.Instance.ChunkSize, World.Instance.ChunkHeight, World.Instance.ChunkSize));
            }
        }
    }
#endif
}