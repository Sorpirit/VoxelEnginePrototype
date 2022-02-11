using System;
using Data;
using UnityEngine;

public class VoxelManipulator : MonoBehaviour
{
    [SerializeField] private BlockType TargetBlock;
    [SerializeField] private World World;
    [SerializeField] private Transform PlacePoints;

    [SerializeField] private int brushSize = 1;

    private Vector3 _previouslyHighlight;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < brushSize * brushSize * brushSize; i++)
            {
                int x = i % brushSize;
                int y = (i / brushSize) % brushSize;
                int z = i / (brushSize * brushSize);
                World.SetBlock(PlacePoints.position + new Vector3(x, y, z) - Vector3.one * brushSize / 2, TargetBlock);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            World.SetBlock(PlacePoints.position, BlockType.Air);
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit))
        {
            PlacePoints.position = raycastHit.point;
        }
        else
        {
            PlacePoints.localPosition = Vector3.forward * 10;
        }
    }
}