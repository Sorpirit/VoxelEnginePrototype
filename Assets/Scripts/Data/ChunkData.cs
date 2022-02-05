using UnityEngine;

namespace Data
{
    public class ChunkData
    {
        public readonly BlockType[] Blocks;
        public readonly int ChunkSize = 16;
        public readonly int ChunkHeight = 100;
        public readonly World WorldReference;
        public Vector3Int WorldPosition;

        public bool ModifiedByThePlayer = false;

        public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
        {
            ChunkHeight = chunkHeight;
            ChunkSize = chunkSize;
            WorldReference = world;
            WorldPosition = worldPosition;
            Blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
        }

    }
}