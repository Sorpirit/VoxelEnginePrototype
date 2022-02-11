using UnityEngine;

namespace Data
{
    public class ChunkData
    {
        public readonly bool IsValid;
        public readonly BlockType[] Blocks;
        public readonly Vector3Int WorldPosition;

        public bool ModifiedByThePlayer;

        public ChunkData(int chunkSize, int chunkHeight, Vector3Int worldPosition)
        {
            WorldPosition = worldPosition;
            Blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
            ModifiedByThePlayer = false;
            IsValid = true;
        }

    }
}