using System.Collections.Generic;
using Data;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    
    public static BlockDataManager Instance { get; private set; }

    [SerializeField]
    private BlockDataSO textureData;
    
    public readonly Dictionary<BlockType, TextureData> BlockTextureDataDictionary = new Dictionary<BlockType, TextureData>();
    public readonly float TextureOffset = 0.001f;
    
    public float TileSizeX { get; private set; }
    public float TileSizeY { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        foreach (var item in textureData.TextureDataList)
        {
            if (BlockTextureDataDictionary.ContainsKey(item.BlockType) == false)
            {
                BlockTextureDataDictionary.Add(item.BlockType, item);
            };
        }
        TileSizeX = textureData.TextureSizeX;
        TileSizeY = textureData.TextureSizeY;
    }
}