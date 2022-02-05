using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BlockData", menuName = "VoxelEngine/Block data", order = 0)]
    public class BlockDataSO : ScriptableObject
    {
        public float TextureSizeX;
        public float TextureSizeY;
        public List<TextureData> TextureDataList;
    }

    [Serializable]
    public class TextureData
    {
        public BlockType BlockType;
        public Vector2Int Up;
        public Vector2Int Down;
        public Vector2Int Side;
        public bool IsSolid = true;
        public bool GeneratesCollider = true;
    }
}