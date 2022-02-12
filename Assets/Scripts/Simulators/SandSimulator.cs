using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UnityTemplateProjects.Simulators
{
    public class SandSimulator : ISimulator
    {
        public BlockType TargetType => BlockType.Sand;
        
        public void SimulateBlock(ChunkData currentChunk, Vector3Int blockPosition, Dictionary<Vector3Int, ChunkRenderer> chunkDictionary)
        {
            if(blockPosition.y <= 0)
            {
                return;
            }

            var downCoords = BlockHelper.GetNeighboursPosition(blockPosition, Direction.Down);
            var downFCoords = BlockHelper.GetNeighboursPosition(downCoords, Direction.Forward);
            var downBCoords = BlockHelper.GetNeighboursPosition(downCoords, Direction.Backwards);
            var downRCoords = BlockHelper.GetNeighboursPosition(downCoords, Direction.Right);
            var downLCoords = BlockHelper.GetNeighboursPosition(downCoords, Direction.Left);
                        
            if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downCoords).IsEmpty())
            {
                Chunk.SwapWithNeighbour(currentChunk, blockPosition, Direction.Down);
            }
            else if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downCoords) != BlockType.Sand)
            {
                return;
            }
            else if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downFCoords).IsEmpty())
            {
                Chunk.SwapBlocks(currentChunk, blockPosition, downFCoords);
            }
            else if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downBCoords).IsEmpty())
            {
                Chunk.SwapBlocks(currentChunk, blockPosition, downBCoords);
            }
            else if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downRCoords).IsEmpty())
            {
                Chunk.SwapBlocks(currentChunk, blockPosition, downRCoords);
            }
            else if (Chunk.GetBlockTypeByCoordsInChunk(currentChunk, downLCoords).IsEmpty())
            {
                Chunk.SwapBlocks(currentChunk, blockPosition, downLCoords);
            }
        }
    }
}