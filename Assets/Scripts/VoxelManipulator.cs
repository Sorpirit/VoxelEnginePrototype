using System;
using Data;
using UnityEngine;

public class VoxelManipulator : MonoBehaviour
{
    [SerializeField] private BlockType TargetBlock;
    [SerializeField] private World World;
    [SerializeField] private Transform PlacePoints;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1))
        {
            SetBlock();
        }
    }

    public void SetBlock()
    {
        World.SetBlock(PlacePoints.position, TargetBlock);
    }
}
