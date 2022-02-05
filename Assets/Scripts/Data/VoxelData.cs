using UnityEngine;

namespace Data
{
    public struct VoxelData
    {
        public Vector3Int Position;
        public Color Color;

        public ParticleSystem.Particle Convert(float scale, float voxelScale)
        {
            return new ParticleSystem.Particle()
            {
                position = (Vector3) Position * scale,
                startColor = Color,
                startSize = voxelScale
            };
        }
    }
}