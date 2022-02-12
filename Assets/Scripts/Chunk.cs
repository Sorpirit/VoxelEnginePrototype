using System;
using Data;
using UnityEngine;

public static class Chunk
{
    public static void LoopThroughTheBlocks(in ChunkData chunkData, Action<Vector3Int> actionToPerform)
    {
        for (int index = 0; index < chunkData.Blocks.Length; index++)
        {
            var position = IndexToPosition(index);
            actionToPerform(position);
        }
    }

    public static void LoopThroughTheBlocks(in ChunkData chunkData, Action<Vector3Int, BlockType> actionToPerform)
    {
        for (int index = 0; index < chunkData.Blocks.Length; index++)
        {
            var position = IndexToPosition(index);
            actionToPerform(position, chunkData.Blocks[index]);
        }
    }

    public static void SwapBlocks(in ChunkData chunkData, Vector3Int currentPos, Vector3Int newPos)
    {
        var typeA = GetBlockTypeByCoordsInChunk(chunkData, currentPos);
        var typeB = GetBlockTypeByCoordsInChunk(chunkData, newPos);
        SetBlock(in chunkData, currentPos, typeB);
        SetBlock(in chunkData, newPos, typeA);
    }

    public static void SwapWithNeighbour(in ChunkData chunkData, Vector3Int currentPos, Direction direction)
    {
        SwapBlocks(in chunkData, currentPos, BlockHelper.GetNeighboursPosition(currentPos, direction));
    }

    /// <summary>
    /// Converts voxel index to voxel chunk position
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Voxel position in chunk coordinates</returns>
    public static Vector3Int IndexToPosition(int index)
    {
        int x = index % World.Instance.ChunkSize;
        int y = (index / World.Instance.ChunkSize) % World.Instance.ChunkHeight;
        int z = index / (World.Instance.ChunkSize * World.Instance.ChunkHeight);
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Converts voxel local position to voxel index
    /// </summary>
    /// <param name="voxelPosition"></param>
    /// <returns></returns>
    public static int PositionToIndex(Vector3Int voxelPosition)
    {
        return voxelPosition.x + World.Instance.ChunkSize * voxelPosition.y +
               World.Instance.ChunkSize * World.Instance.ChunkHeight * voxelPosition.z;
    }

    //in chunk coordinate system
    private static bool InRange(int axisCoordinate)
    {
        if (axisCoordinate < 0 || axisCoordinate >= World.Instance.ChunkSize)
            return false;

        return true;
    }

    //in chunk coordinate system
    private static bool InRangeHeight(int yCoordinate)
    {
        if (yCoordinate < 0 || yCoordinate >= World.Instance.ChunkHeight)
            return false;

        return true;
    }

    public static BlockType GetBlockTypeByCoordsInChunk(in ChunkData chunkData, Vector3Int voxelPosition)
    {
        if (InRange(voxelPosition.x) && InRangeHeight(voxelPosition.y) &&
            InRange(voxelPosition.z))
        {
            int index = PositionToIndex(voxelPosition);
            return chunkData.Blocks[index];
        }

        //TODO add some assert in this case or test it
        return BlockType.Nothing;
    }

    public static void SetBlock(in ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (InRange(localPosition.x) && InRangeHeight(localPosition.y) &&
            InRange(localPosition.z))
        {
            int index = PositionToIndex(localPosition);
            chunkData.Blocks[index] = block;
        }
        else
        {
            throw new Exception("Need to ask World for appropiate chunk");
        }
    }
    
    public static Vector3Int WorldToLocal(in ChunkData chunkData, Vector3 voxelWorldPosition)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(voxelWorldPosition.x - chunkData.WorldPosition.x),
            y = Mathf.FloorToInt(voxelWorldPosition.y - chunkData.WorldPosition.y),
            z = Mathf.FloorToInt(voxelWorldPosition.z - chunkData.WorldPosition.z) 
        };
        return pos;
    }

    public static void LoadMeshData(in ChunkData chunkData, in MeshData meshData)
    {
        Debug.Assert(meshData.IsValid, "Mesh data should be valid on this step");
        for (int index = 0; index < chunkData.Blocks.Length; index++)
        {
            var position = IndexToPosition(index);
            BlockHelper.LoadVoxelGeometryData(in chunkData, in meshData, position);
        }
    }

    public static Vector3Int WorldToChunkPosition(Vector3 worldPosition)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(worldPosition.x / World.Instance.ChunkSize) * World.Instance.ChunkSize,
            y = Mathf.FloorToInt(worldPosition.y / World.Instance.ChunkHeight) * World.Instance.ChunkHeight,
            z = Mathf.FloorToInt(worldPosition.z / World.Instance.ChunkSize) * World.Instance.ChunkSize
        };
        return pos;
    }
}