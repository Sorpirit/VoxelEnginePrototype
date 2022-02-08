using System.Collections.Generic;
using Data;
using Helpers;
using UnityEngine;

public class World : MonoBehaviour
{
    public int MapSizeInChunks = 6;
    public int ChunkSize = 16;
    public int ChunkHeight = 100;
    public float NoiseScale = 0.03f;
    public GameObject ChunkPrefab;

    private readonly Dictionary<Vector3Int, ChunkData> _chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    private readonly Dictionary<Vector3Int, ChunkRenderer> _chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void GenerateWorld()
    {
        _chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in _chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        _chunkDictionary.Clear();

        for (int x = 0; x < MapSizeInChunks; x++)
        {
            for (int z = 0; z < MapSizeInChunks; z++)
            {

                ChunkData data = new ChunkData(ChunkSize, ChunkHeight, this, new Vector3Int(x * ChunkSize, 0, z * ChunkSize));
                GenerateVoxels(data);
                _chunkDataDictionary.Add(data.WorldPosition, data);
            }
        }

        foreach (ChunkData data in _chunkDataDictionary.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(ChunkPrefab, data.WorldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            _chunkDictionary.Add(data.WorldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.ChunkSize; x++)
        {
            for (int z = 0; z < data.ChunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.WorldPosition.x + x) * NoiseScale, (data.WorldPosition.z + z) * NoiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * ChunkHeight);
                for (int y = 0; y < ChunkHeight; y++)
                {
                    BlockType voxelType = BlockType.Rock;
                    if (y > groundPosition)
                    {
                        voxelType = BlockType.Air;
                    }
                    else if (y == groundPosition)
                    {
                        voxelType = BlockType.Sand;
                    }

                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    public void SetBlock(Vector3 worldPos, BlockType newType)
    {
        Vector3Int pos = Chunk.GetChunkPosition(this, worldPos);

        if (!_chunkDictionary.TryGetValue(pos, out ChunkRenderer containerChunk))
            return;

        Vector3Int blockInChunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk.ChunkData, worldPos.ToInt());
        var prevType = Chunk.GetBlockFromChunkCoordinates(containerChunk.ChunkData, blockInChunkCoordinates);
        if (newType != prevType)
        {
            Chunk.SetBlock(containerChunk.ChunkData, blockInChunkCoordinates, newType);
            containerChunk.UpdateChunk();
        }
    }

    public BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        
        if(!_chunkDataDictionary.TryGetValue(pos, out ChunkData containerChunk))
            return BlockType.Nothing;

        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }
}