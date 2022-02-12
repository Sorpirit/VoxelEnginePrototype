using Data;
using UnityEngine;

public static class BlockHelper
{
    private static Direction[] _directions =
    {
        Direction.Backwards,
        Direction.Down,
        Direction.Forward,
        Direction.Left,
        Direction.Right,
        Direction.Up
    };

    private static Vector2[] _uv = new Vector2[4];

    /// <summary>
    /// Inserts geometry data into <see cref="MeshData"/> using voxel position 
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="voxelPosition"></param>
    /// <param name="meshData"></param>
    /// <returns></returns>
    public static void LoadVoxelGeometryData(in ChunkData chunk, in MeshData meshData, Vector3Int voxelPosition)
    {
        var blockType = Chunk.GetBlockTypeByCoordsInChunk(chunk, voxelPosition);
        
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return;

        foreach (Direction direction in _directions)
        {
            var neighbourBlockCoordinates = voxelPosition + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockTypeByCoordsInChunk(in chunk, neighbourBlockCoordinates);

            if (neighbourBlockType != BlockType.Nothing && !BlockDataManager.Instance.BlockTextureDataDictionary[neighbourBlockType].IsSolid)
            {
                GetFaceDataIn(in meshData, direction, voxelPosition, blockType);
            }
        }

        return;
    }

    /// <summary>
    /// Load face of the voxel in specified <see cref="Direction"/>
    /// </summary>
    /// <param name="meshData"></param>
    /// <param name="direction"></param>
    /// <param name="voxelPosition"></param>
    /// <param name="blockType"></param>
    public static void GetFaceDataIn(in MeshData meshData, Direction direction, Vector3Int voxelPosition, BlockType blockType)
    {
        GetFaceVertices(direction, voxelPosition, in meshData);
        meshData.AddQuadTriangles();
        meshData.UV.AddRange(FaceUVs(direction, blockType));
    }
    
    public static void GetFaceVertices(Direction direction, Vector3Int voxelPosition, in MeshData meshData)
    {
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.Backwards:
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                break;
            case Direction.Forward:
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                break;
            case Direction.Left:
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                break;

            case Direction.Right:
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                break;
            case Direction.Down:
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y - 0.5f, voxelPosition.z + 0.5f));
                break;
            case Direction.Up:
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z + 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x + 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                meshData.AddVertex(new Vector3(voxelPosition.x - 0.5f, voxelPosition.y + 0.5f, voxelPosition.z - 0.5f));
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        var tilePos = TexturePosition(direction, blockType);

        _uv[0] = new Vector2(BlockDataManager.Instance.TileSizeX * tilePos.x + BlockDataManager.Instance.TileSizeX - BlockDataManager.Instance.TextureOffset,
            BlockDataManager.Instance.TileSizeY * tilePos.y + BlockDataManager.Instance.TextureOffset);

        _uv[1] = new Vector2(BlockDataManager.Instance.TileSizeX * tilePos.x + BlockDataManager.Instance.TileSizeX - BlockDataManager.Instance.TextureOffset,
            BlockDataManager.Instance.TileSizeY * tilePos.y + BlockDataManager.Instance.TileSizeY - BlockDataManager.Instance.TextureOffset);

        _uv[2] = new Vector2(BlockDataManager.Instance.TileSizeX * tilePos.x + BlockDataManager.Instance.TextureOffset,
            BlockDataManager.Instance.TileSizeY * tilePos.y + BlockDataManager.Instance.TileSizeY - BlockDataManager.Instance.TextureOffset);

        _uv[3] = new Vector2(BlockDataManager.Instance.TileSizeX * tilePos.x + BlockDataManager.Instance.TextureOffset,
            BlockDataManager.Instance.TileSizeY * tilePos.y + BlockDataManager.Instance.TextureOffset);

        return _uv;
    }

    public static Vector2Int TexturePosition(Direction direction, BlockType blockType)
    {
        return direction switch
        {
            Direction.Up => BlockDataManager.Instance.BlockTextureDataDictionary[blockType].Up,
            Direction.Down => BlockDataManager.Instance.BlockTextureDataDictionary[blockType].Down,
            _ => BlockDataManager.Instance.BlockTextureDataDictionary[blockType].Side
        };
    }
    
    public static Vector3Int GetNeighboursPosition(Vector3Int pos, Direction direction)
    {
        //TODO make scale relative
        switch (direction)
        {
            case Direction.Backwards:
                return pos + Vector3Int.back;
            case Direction.Forward:
                return pos + Vector3Int.forward;
            case Direction.Left:
                return pos + Vector3Int.left;
            case Direction.Right:
                return pos + Vector3Int.right;
            case Direction.Up:
                return pos + Vector3Int.up;
            case Direction.Down:
                return pos + Vector3Int.down;
        }

        return pos;
    }
}