using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/10/2025]
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
    /// returns a vector 2 for each enum
    /// </summary>
    /// <param name="direction">enum of direction</param>
    /// <returns>Vector2</returns>
    public Vector2I GetVectorIDirection(DirectionEnum direction)
    {
        Vector2I dir = new Vector2I();
        switch (direction)
        {
            case DirectionEnum.UP:
                dir = Vector2I.Up;
                break;
            case DirectionEnum.DOWN:
                dir = Vector2I.Down;
                break;
            case DirectionEnum.LEFT:
                dir = Vector2I.Left;
                break;
            case DirectionEnum.RIGHT:
                dir = Vector2I.Right;
                break;
        }
        return dir;
    }

    /// <summary>
    /// returns the float value for the DEGREE rotation of enum
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

    public DirectionEnum GetOppositeDirection(DirectionEnum direction)
    {
        switch (direction)
        {
            case DirectionEnum.UP:
                return DirectionEnum.DOWN;
            case DirectionEnum.DOWN:
                return DirectionEnum.UP;
            case DirectionEnum.LEFT:
                return DirectionEnum.RIGHT;
            case DirectionEnum.RIGHT:
                return DirectionEnum.LEFT;
        }
        //some how
        return 0;
    }

    /// <summary>
    /// gets the closest direction of 2 coordinates
    /// 
    /// NOTE: if it is a 50/50 split, it will send 2
    /// </summary>
    /// <param name="start">first coord</param>
    /// <param name="end">second coord</param>
    /// <returns>array of direction enums</returns>
    public DirectionEnum[] GetClosestDirection(Vector2 start, Vector2 end)
    {
        List<DirectionEnum> output = new List<DirectionEnum>();

        Vector2 direction = start.DirectionTo(end);

        int searchXAxis = 0;
        int searchBoth = 1;
        int searchYAxis = 2;

        int searchType = 1;

        if (Mathf.IsEqualApprox(direction.X, direction.Y))
        {
            searchType = searchBoth;
        }
        else if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            searchType = searchXAxis;
        }
        else
        {
            searchType = searchYAxis;
        }


        if (searchType <= searchBoth)
        {
            if (direction.X > 0)
            {
                output.Add(DirectionEnum.RIGHT);
            }
            else if (direction.X < 0)
            {
                output.Add(DirectionEnum.LEFT);
            }
        }

        if (searchType >= searchBoth)
        {
            if (direction.Y > 0)
            {
                output.Add(DirectionEnum.DOWN);
            }
            else if (direction.Y < 0)
            {
                output.Add(DirectionEnum.UP);
            }
        }

        return output.ToArray();
    }

    public static DirectionManager Instance
    {
        get { return _instance; }
    }
}
