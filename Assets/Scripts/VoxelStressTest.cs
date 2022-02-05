using System;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityTemplateProjects
{
    public class VoxelStressTest : MonoBehaviour
    {
        [SerializeField] private VoxelRenderer _renderer;
        [SerializeField] private Vector3Int fillCube = new Vector3Int(1000, 1000, 1000);
        [SerializeField] private int count = 10000;
        
        private void Start()
        {
            VoxelData[] data = new VoxelData[count];

            for (int i = 0; i < data.Length; i++)
            {
                data[i].Position.x = Random.Range(0, fillCube.x);
                data[i].Position.y = Random.Range(0, fillCube.y);
                data[i].Position.z = Random.Range(0, fillCube.z);
                data[i].Color = new Color(Random.value, Random.value, Random.value);
            }
            
            _renderer.SetVoxels(data);
        }
    }
}