using System.Collections.Generic;
using Data;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static readonly Dictionary<BlockType, TextureData> BlockTextureDataDictionary = new Dictionary<BlockType, TextureData>();
    public static readonly float TextureOffset = 0.001f;
    public static float TileSizeX;
    public static float TileSizeY;
    public BlockDataSO TextureData;

    private void Awake()
    {
        foreach (var item in TextureData.TextureDataList)
        {
            if (BlockTextureDataDictionary.ContainsKey(item.BlockType) == false)
            {
                BlockTextureDataDictionary.Add(item.BlockType, item);
            };
        }
        TileSizeX = TextureData.TextureSizeX;
        TileSizeY = TextureData.TextureSizeY;
    }
}