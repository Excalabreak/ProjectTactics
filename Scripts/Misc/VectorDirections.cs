using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/25/2025]
 * [singleton that gets direction for each direction enum]
 */

public partial class VectorDirections : Node2D
{
    private static VectorDirections _instance;

    public override void _Ready()
    {
        if (_instance!= null)
        {
            QueueFree();
            return;
        }

        _instance = this;
    }

    /// <summary>
    /// returns a vector 2 for each enum
    /// </summary>
    /// <param name="direction">enum of direction</param>
    /// <returns>Vector2</returns>
    public Vector2 GetDirection(Direction direction)
    {
        Vector2 dir = new Vector2();
        switch (direction)
        {
            case Direction.UP:
                dir = Vector2.Up;
                break;
            case Direction.DOWN:
                dir = Vector2.Down;
                break;
            case Direction.LEFT:
                dir = Vector2.Left;
                break;
            case Direction.RIGHT:
                dir = Vector2.Right;
                break;
        }
        return dir;
    }

    public static VectorDirections Instance
    {
        get { return _instance; }
    }
}
