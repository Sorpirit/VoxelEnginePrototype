using System;
using Data;
using UnityEngine;

public class VoxelManipulator : MonoBehaviour
{
    [SerializeField] private BlockType TargetBlock;
    [SerializeField] private World World;
    [SerializeField] private Transform PlacePoints;

    private Vector3 _previouslyHighlight;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            World.SetBlock(PlacePoints.position, TargetBlock);
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
