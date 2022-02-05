using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VoxelRenderer : MonoBehaviour
{

    [SerializeField]
    private float voxelScale = 0.1f;
    [SerializeField]
    private float scale = 1f;
    
    private ParticleSystem _system;
    private ParticleSystem.Particle[] _voxels;
    private bool _voxelsUpdated = false;
    
    private void Start()
    {
        _system = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_voxelsUpdated)
        {
            _system.SetParticles(_voxels, _voxels.Length);
            _voxelsUpdated = false;
        }
    }

    public void SetVoxels(VoxelData[] data)
    {
        _voxels = new ParticleSystem.Particle[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            _voxels[i] = data[i].Convert(scale, voxelScale);
        }
        
        Debug.Log("Voxels set! Voxel count:" + _voxels.Length);
        _voxelsUpdated = true;
    }
}
