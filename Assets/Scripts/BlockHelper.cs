using Data;
using UnityEngine;

public static class BlockHelper
{
    private static Direction[] directions =
    {
        Direction.Backwards,
        Direction.Down,
        Direction.Foreward,
        Direction.Left,
        Direction.Right,
        Direction.Up
    };

    public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);

            if (neighbourBlockType != BlockType.Nothing && !BlockDataManager.BlockTextureDataDictionary[neighbourBlockType].IsSolid)
            {
                meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, blockType);
            }
        }

        return meshData;
    }

    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        GetFaceVertices(direction, x, y, z, meshData, blockType);
        meshData.AddQuadTriangles(BlockDataManager.BlockTextureDataDictionary[blockType].GeneratesCollider);
        meshData.UV.AddRange(FaceUVs(direction, blockType));
        
        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        var generatesCollider = BlockDataManager.BlockTextureDataDictionary[blockType].GeneratesCollider;
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.Backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.Foreward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.Left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;

            case Direction.Right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.Down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.Up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, blockType);

        UVs[0] = new Vector2(BlockDataManager.TileSizeX * tilePos.x + BlockDataManager.TileSizeX - BlockDataManager.TextureOffset,
            BlockDataManager.TileSizeY * tilePos.y + BlockDataManager.TextureOffset);

        UVs[1] = new Vector2(BlockDataManager.TileSizeX * tilePos.x + BlockDataManager.TileSizeX - BlockDataManager.TextureOffset,
            BlockDataManager.TileSizeY * tilePos.y + BlockDataManager.TileSizeY - BlockDataManager.TextureOffset);

        UVs[2] = new Vector2(BlockDataManager.TileSizeX * tilePos.x + BlockDataManager.TextureOffset,
            BlockDataManager.TileSizeY * tilePos.y + BlockDataManager.TileSizeY - BlockDataManager.TextureOffset);

        UVs[3] = new Vector2(BlockDataManager.TileSizeX * tilePos.x + BlockDataManager.TextureOffset,
            BlockDataManager.TileSizeY * tilePos.y + BlockDataManager.TextureOffset);

        return UVs;
    }

    public static Vector2Int TexturePosition(Direction direction, BlockType blockType)
    {
        return direction switch
        {
            Direction.Up => BlockDataManager.BlockTextureDataDictionary[blockType].Up,
            Direction.Down => BlockDataManager.BlockTextureDataDictionary[blockType].Down,
            _ => BlockDataManager.BlockTextureDataDictionary[blockType].Side
        };
    }
}