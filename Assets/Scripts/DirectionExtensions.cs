using System;
using Data;
using UnityEngine;

public static class DirectionExtensions
{
    public static Vector3Int GetVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Vector3Int.up,
            Direction.Down => Vector3Int.down,
            Direction.Right => Vector3Int.right,
            Direction.Left => Vector3Int.left,
            Direction.Foreward => Vector3Int.forward,
            Direction.Backwards => Vector3Int.back,
            _ => throw new Exception("Invalid input direction")
        };
    }
}