using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Data;
using UnityEngine;
using UnityTemplateProjects.Simulators;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    public int MapSizeInChunks = 6;
    public int ChunkSize = 16;
    public int ChunkHeight = 100;
    public float NoiseScale = 0.03f;
    public GameObject ChunkPrefab;
    public float UpdateRate;

    private readonly Dictionary<Vector3Int, ChunkRenderer> _chunkDictionary =
        new Dictionary<Vector3Int, ChunkRenderer>();

    private Dictionary<BlockType, ISimulator> _simulators;


    private float _simulationTimer;

    private void Awake()
    {
        Instance = this;
        InitSimulators();
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

        Chunk.SetBlock(_chunkDictionary[Vector3Int.zero].ChunkData, new Vector3Int(ChunkSize / 2, 1, ChunkSize / 2),
            BlockType.Rock);
        Chunk.SetBlock(_chunkDictionary[Vector3Int.zero].ChunkData, new Vector3Int(ChunkSize / 2, 1, ChunkSize / 2),
            BlockType.Air);
    }

    private void Update()
    {
        if (_simulationTimer <= 0)
        {
            foreach (var chunkRend in _chunkDictionary.Values)
            {
                chunkRend.UpdateChunk();
            }

            Simulate();
            _simulationTimer = UpdateRate;
        }

        _simulationTimer -= Time.deltaTime;
    }

    public Vector3Int GetGlobalCoordsByLocal(ChunkData currentChunk, Vector3Int voxelCoords)
    {
        if (currentChunk is null)
        {
            throw new ArgumentNullException();
        }

        // TODO: Check if coords are correct

        return voxelCoords + currentChunk.WorldPosition;
    }


    public (ChunkData localChunk, Vector3Int localCoords) GetLocalCoordsByWorldCoords(Vector3Int globalVoxelCoords)
    {
        ChunkData localChunk = _chunkDictionary[
            new Vector3Int(
                globalVoxelCoords.x / ChunkSize * ChunkSize,
                0,
                globalVoxelCoords.z / ChunkSize * ChunkSize)
        ].ChunkData;

        Vector3Int localCoords = globalVoxelCoords - localChunk.WorldPosition;

        return (localChunk, localCoords);
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

    public void SetBlock(Vector3 worldPos, BlockType newType)
    {
        Vector3Int chunkPosition = Chunk.WorldToChunkPosition(worldPos);

        if (!_chunkDictionary.TryGetValue(chunkPosition, out ChunkRenderer containerChunk))
            return;

        Vector3Int blockInChunkCoordinates = Chunk.WorldToLocal(containerChunk.ChunkData, worldPos);
        var prevType = Chunk.GetBlockTypeByCoordsInChunk(containerChunk.ChunkData, blockInChunkCoordinates);
        if (newType != prevType)
        {
            Chunk.SetBlock(containerChunk.ChunkData, blockInChunkCoordinates, newType);
            containerChunk.UpdateChunk();
        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.WorldPosition.x + x) * NoiseScale,
                    (data.WorldPosition.z + z) * NoiseScale);
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

    private void Simulate()
    {
        foreach (var chunkRenderer in _chunkDictionary.Values)
        {
            Chunk.LoopThroughTheBlocks(chunkRenderer.ChunkData, (pos, type) =>
            {
                if (_simulators.ContainsKey(type))
                {
                    _simulators[type].SimulateBlock(chunkRenderer.ChunkData, pos, _chunkDictionary);
                }
            });
        }
    }

    private void InitSimulators()
    {
        _simulators = new Dictionary<BlockType, ISimulator>
        {
            {BlockType.Sand, new SandSimulator()},
            {BlockType.Water, new WaterSimulator()}
        };
    }
}