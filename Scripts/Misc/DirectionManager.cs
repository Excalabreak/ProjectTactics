using Godot;
using System;
using static Godot.TextServer;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/05/2025]
 * [cant make vector 2 const array, 
 * so this was my next idea, 
 * now it also deals in rotation]
 */

public partial class DirectionManager : Node2D
{
    private static DirectionManager _instance;

    public override void _Ready()
    {
        if (_instance != null)
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
    public Vector2 GetVectorDirection(DirectionEnum direction)
    {
        Vector2 dir = new Vector2();
        switch (direction)
        {
            case DirectionEnum.UP:
                dir = Vector2.Up;
                break;
            case DirectionEnum.DOWN:
                dir = Vector2.Down;
                break;
            case DirectionEnum.LEFT:
                dir = Vector2.Left;
                break;
            case DirectionEnum.RIGHT:
                dir = Vector2.Right;
                break;
        }
        return dir;
    }

    /// <summary>
    /// returns the float value for the rotation of enum
    /// </summary>
    /// <param name="direction">direction enum</param>
    /// <returns>float of rotation</returns>
    public float GetRotationDirection(DirectionEnum direction)
    {
        switch (direction)
        {
            case DirectionEnum.UP:
                return 0f;
            case DirectionEnum.DOWN:
                return 180f;
            case DirectionEnum.LEFT:
                return 270f;
            case DirectionEnum.RIGHT:
                return 90f;
        }
        //some how
        return 0;
    }

    public static DirectionManager Instance
    {
        get { return _instance; }
    }
}
