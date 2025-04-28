using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [04/24/2025]
 * [pathfinding for units]
 */

public partial class UnitPath : TileMapLayer
{
    [Export] private GridResource _grid;

    private Pathfinder _pathfinder;

    private Vector2[] _currentPath;

    //test path
    /*
    public override void _Ready()
    {
        Vector2 start = new Vector2(0, 0);
        Vector2 end = new Vector2(6, 3);

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i <= end.X - start.X; i++)
        {
            for (int j = 0; j <= end.Y - start.Y; j++)
            {
                points.Add(start + new Vector2(i, j));
            }
        }

        Initialize(points.ToArray());
        DrawPath(start, new Vector2(3, 1));
    }
    */

    /// <summary>
    /// initializes path
    /// </summary>
    /// <param name="walkableCells">cells to walk through</param>
    public void Initialize(Vector2[] walkableCells)
    {
        _pathfinder = new Pathfinder(_grid, walkableCells);
    }

    /// <summary>
    /// draws path on tile map
    /// </summary>
    /// <param name="cellStart">start location</param>
    /// <param name="cellEnd">last location</param>
    public void DrawPath(Vector2 cellStart, Vector2 cellEnd)
    {
        Clear();
        _currentPath = _pathfinder.CalculatePointPath(cellStart, cellEnd);

        Vector2I[] path = new Vector2I[_currentPath.Length];
        for (int i = 0; i < _currentPath.Length; i++)
        {
            path[i] = new Vector2I((int)Mathf.Round(_currentPath[i].X), (int)Mathf.Round(_currentPath[i].Y));
        }
        SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I>(path),0,0);
    }

    /// <summary>
    /// stops drawing path and clears the path
    /// </summary>
    public void Stop()
    {
        _pathfinder = null;
        Clear();
    }

    //properties

    public Vector2[] currentPath
    {
        get { return _currentPath; }
    }
}
