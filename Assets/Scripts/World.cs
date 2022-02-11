using System;
using System.Collections.Generic;
using Data;
using Helpers;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
    
    public int MapSizeInChunks = 6;
    public int ChunkSize = 16;
    public int ChunkHeight = 100;
    public float NoiseScale = 0.03f;
    public GameObject ChunkPrefab;
    public float UpdateRate;

    private readonly Dictionary<Vector3Int, ChunkRenderer> _chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    private float _simulationTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int x = 0; x < MapSizeInChunks; x++)
        {
            for (int z = 0; z < MapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x * ChunkSize, 0, z * ChunkSize));
                GameObject chunkObject = Instantiate(ChunkPrefab, data.WorldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                _chunkDictionary.Add(data.WorldPosition, chunkRenderer);
                GenerateEmptyVoxels(data);
                chunkRenderer.ChunkData = data;
            }
        }
        Chunk.SetBlock(_chunkDictionary[Vector3Int.zero].ChunkData, new Vector3Int(ChunkSize / 2, 1, ChunkSize / 2), BlockType.Rock);
        Chunk.SetBlock(_chunkDictionary[Vector3Int.zero].ChunkData, new Vector3Int(ChunkSize / 2, 1, ChunkSize / 2), BlockType.Air);
    }

    private void Update()
    {
        if (_simulationTimer <= 0)
        {
            Simulate();
            foreach (var chunkRend in _chunkDictionary.Values)
            {
                chunkRend.UpdateChunk();
            }
            _simulationTimer = UpdateRate;
        }

        _simulationTimer -= Time.deltaTime;
    }

    public void GenerateWorld()
    {
        foreach (ChunkRenderer chunk in _chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        _chunkDictionary.Clear();

        for (int x = 0; x < MapSizeInChunks; x++)
        {
            for (int z = 0; z < MapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x * ChunkSize, 0, z * ChunkSize));
                GenerateVoxels(data);
                GameObject chunkObject = Instantiate(ChunkPrefab, data.WorldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                _chunkDictionary.Add(data.WorldPosition, chunkRenderer);
                chunkRenderer.ChunkData = data;
            }
        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
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
    
    private void GenerateEmptyVoxels(in ChunkData data)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    BlockType voxelType = BlockType.Air;
                    Chunk.SetBlock(in data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    public void SetBlock(Vector3 worldPos, BlockType newType)
    {
        Vector3Int chunkPosition = Chunk.WorldToChunkPosition(worldPos);

        if (!_chunkDictionary.TryGetValue(chunkPosition, out ChunkRenderer containerChunk))
            return;

        Vector3Int blockInChunkCoordinates = Chunk.WorldToLocal(containerChunk.ChunkData, worldPos);
        var prevType = Chunk.GetVoxelTypeChunkSpace(containerChunk.ChunkData, blockInChunkCoordinates);
        if (newType != prevType)
        {
            Chunk.SetBlock(containerChunk.ChunkData, blockInChunkCoordinates, newType);
            containerChunk.UpdateChunk();
        }
    }
    
    private void Simulate()
    {
        foreach (var chunkRenderer in _chunkDictionary.Values)
        {
            Chunk.LoopThroughTheBlocks(chunkRenderer.ChunkData, (pos, type) =>
            {
                switch (type)
                {
                    case BlockType.Sand:
                        if(pos.y <= 0)
                            return;

                        var down = BlockHelper.GetNeighboursPosition(pos, Direction.Down);
                        var downF = BlockHelper.GetNeighboursPosition(down, Direction.Forward);
                        var downB = BlockHelper.GetNeighboursPosition(down, Direction.Backwards);
                        var downR = BlockHelper.GetNeighboursPosition(down, Direction.Right);
                        var downL = BlockHelper.GetNeighboursPosition(down, Direction.Left);
                        
                        if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, down).IsEmpty())
                        {
                            Chunk.SwapWithNeighbour(chunkRenderer.ChunkData, pos, Direction.Down);
                        }
                        else if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, down) != BlockType.Sand)
                        {
                            return;
                        }
                        else if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, downF).IsEmpty())
                        {
                            Chunk.SwapBlocks(chunkRenderer.ChunkData, pos, downF);
                        }
                        else if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, downB).IsEmpty())
                        {
                            Chunk.SwapBlocks(chunkRenderer.ChunkData, pos, downB);
                        }
                        else if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, downR).IsEmpty())
                        {
                            Chunk.SwapBlocks(chunkRenderer.ChunkData, pos, downR);
                        }
                        else if (Chunk.GetVoxelTypeChunkSpace(chunkRenderer.ChunkData, downL).IsEmpty())
                        {
                            Chunk.SwapBlocks(chunkRenderer.ChunkData, pos, downL);
                        }
                        break;
                }
            });
        }
    }
}