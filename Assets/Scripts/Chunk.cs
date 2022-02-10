using System;
using Data;
using UnityEngine;

public static class Chunk
{
    public static void LoopThroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.Blocks.Length; index++)
        {
            var position = GetPostitionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

    public static void LoopThroughTheBlocks(ChunkData chunkData, Action<Vector3Int, BlockType> actionToPerform)
    {
        for (int index = 0; index < chunkData.Blocks.Length; index++)
        {
            var position = GetPostitionFromIndex(chunkData, index);
            actionToPerform(position, chunkData.Blocks[index]);
        }
    }

    public static void SwapBlock(ChunkData chunkData, Vector3Int currentPos, Vector3Int newPos)
    {
        var typeA = GetBlockFromChunkCoordinates(chunkData, currentPos);
        var typeB = GetBlockFromChunkCoordinates(chunkData, newPos);
        SetBlock(chunkData, currentPos, typeB);
        SetBlock(chunkData, newPos, typeA);
    }

    public static void SwapWithNeighbour(ChunkData chunkData, Vector3Int currentPos, Direction direction)
    {
        SwapBlock(chunkData, currentPos, BlockHelper.GetNeighboursPosition(currentPos, direction));
    }

    private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.ChunkSize;
        int y = (index / chunkData.ChunkSize) % chunkData.ChunkHeight;
        int z = index / (chunkData.ChunkSize * chunkData.ChunkHeight);
        return new Vector3Int(x, y, z);
    }

    //in chunk coordinate system
    private static bool InRange(ChunkData chunkData, int axisCoordinate)
    {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.ChunkSize)
            return false;

        return true;
    }

    //in chunk coordinate system
    private static bool InRangeHeight(ChunkData chunkData, int ycoordinate)
    {
        if (ycoordinate < 0 || ycoordinate >= chunkData.ChunkHeight)
            return false;

        return true;
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }
    
    public static BlockType GetNeighbourBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates, Direction neighbour)
    {
        chunkCoordinates = BlockHelper.GetNeighboursPosition(chunkCoordinates, neighbour);
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.Blocks[index];
        }

        return chunkData.WorldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.WorldPosition.x + x, chunkData.WorldPosition.y + y, chunkData.WorldPosition.z + z);
    }
    
    public static void SetChunkBlock(ChunkData chunkData, BlockType newType, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            chunkData.Blocks[index] = newType;
        }
    }
    
    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
        {
            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.Blocks[index] = block;
        }
        else
        {
            throw new Exception("Need to ask World for appropiate chunk");
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.ChunkSize * y + chunkData.ChunkSize * chunkData.ChunkHeight * z;
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.WorldPosition.x,
            y = pos.y - chunkData.WorldPosition.y,
            z = pos.z - chunkData.WorldPosition.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData();

        LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.Blocks[GetIndexFromPosition(chunkData, x, y, z)]));
        
        return meshData;
    }
    
    public static MeshData GetChunkMeshData(ChunkData chunkData, MeshData meshData)
    {
        LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.Blocks[GetIndexFromPosition(chunkData, x, y, z)]));
        return meshData;
    }

    internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.ChunkSize) * world.ChunkSize,
            y = Mathf.FloorToInt(y / (float)world.ChunkHeight) * world.ChunkHeight,
            z = Mathf.FloorToInt(z / (float)world.ChunkSize) * world.ChunkSize
        };
        return pos;
    }
    
    public static Vector3Int GetChunkPosition(World world, Vector3 worldPos)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(worldPos.x / world.ChunkSize) * world.ChunkSize,
            y = Mathf.FloorToInt(worldPos.y / world.ChunkHeight) * world.ChunkHeight,
            z = Mathf.FloorToInt(worldPos.z / world.ChunkSize) * world.ChunkSize
        };
        return pos;
    }
}