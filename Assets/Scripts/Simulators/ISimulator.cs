using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UnityTemplateProjects.Simulators
{
    public interface ISimulator
    {
        BlockType TargetType { get; }

        void SimulateBlock(ChunkData currentChunk, Vector3Int blockPosition,
            Dictionary<Vector3Int, ChunkRenderer> chunkDictionary);
    }
}