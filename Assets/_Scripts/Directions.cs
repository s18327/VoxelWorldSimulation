using System;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    Up,     // +y direction
    Down,    // -y direction
    Left,   // -x direction
    Right,  // +x direction
    Forward,  // z+ direction
    Backwards   // -z direction
};
public static class Directions {

    public static readonly List<Vector2Int> Directions2D = new ()
    {
        new ( 0, 1), //N
        new ( 1, 1), //NE
        new ( 1, 0), //E
        new (-1, 1), //SE
        new (-1, 0), //S
        new (-1,-1), //SW
        new ( 0,-1), //W
        new ( 1,-1)  //NW
    };
    
    public static Vector3Int GetVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Vector3Int.up,
            Direction.Down => Vector3Int.down,
            Direction.Left => Vector3Int.left,
            Direction.Right => Vector3Int.right,
            Direction.Forward => Vector3Int.forward,
            Direction.Backwards => Vector3Int.back,
            _ => throw new Exception("Invalid input direction")
        };
    }
}
