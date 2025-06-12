using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [06/12/2025]
 * [pathfinding for units]
 */

public partial class UnitPath : TileMapLayer
{
    [Export] private GameBoard _gameBoard;

    private Pathfinder _pathfinder;

    private List<Vector2> _currentPath = new List<Vector2>();
    private Vector2 _unitPos;

    /// <summary>
    /// initializes path
    /// </summary>
    /// <param name="walkableCells">cells to walk through</param>
    /// <param name="unitPos">the first coordinate in unit pos</param>
    public void Initialize(Vector2[] walkableCells, Vector2 unitPos)
    {
        _pathfinder = new Pathfinder(_gameBoard.grid, walkableCells);
        _unitPos = unitPos;
        ResetCurrentPath();
    }

    /// <summary>
    /// draws path on tile map automatically
    /// </summary>
    /// <param name="cellStart">start location</param>
    /// <param name="cellEnd">last location</param>
    public void DrawAutoPath(Vector2 cellStart, Vector2 cellEnd)
    {
        Clear();
        _currentPath.Clear();
        _currentPath.AddRange(_pathfinder.CalculatePointPath(cellStart, cellEnd));

        DrawPathLine(_currentPath.ToArray());
    }

    public void AddTileToCurrentPath(Vector2 newCell)
    {
        Clear();
        _currentPath.Add(newCell);

        DrawPathLine(_currentPath.ToArray());
    }

    private void DrawPathLine(Vector2[] pathCoord)
    {
        Vector2I[] path = new Vector2I[pathCoord.Length];
        for (int i = 0; i < pathCoord.Length; i++)
        {
            path[i] = new Vector2I(Mathf.RoundToInt(pathCoord[i].X), Mathf.RoundToInt(pathCoord[i].Y));
        }
        SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I>(path), 0, 0);
    }

    /// <summary>
    /// checks if cell is connected to the last point
    /// on the current path
    /// </summary>
    /// <param name="cell">coordinate of cell</param>
    /// <returns>true if the coord is next to the path</returns>
    public bool CoordConnects(Vector2 cell)
    {
        if (_currentPath.Count == 0)
        {
            return false;
        }
        if (!_gameBoard.grid.IsWithinBounds(cell))
        {
            return false;
        }

        foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
        {
            if ((_currentPath[_currentPath.Count - 1] + DirectionManager.Instance.GetVectorDirection(dir))
                .IsEqualApprox(cell))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// stops drawing path and clears the path
    /// </summary>
    public void Stop()
    {
        _unitPos = new Vector2(-1,-1);
        _pathfinder = null;
        Clear();
    }

    /// <summary>
    /// resets the current path
    /// </summary>
    public void ResetCurrentPath()
    {
        Clear();
        _currentPath.Clear();
        _currentPath.Add(_unitPos);
    }

    /// <summary>
    /// gets a Vector2I version of _currentPath
    /// </summary>
    /// <returns>Vector2I[] of current path</returns>
    public Vector2I[] GetIntCurrentPath()
    {
        Vector2I[] path = new Vector2I[_currentPath.Count];
        for (int i = 0; i < _currentPath.Count; i++)
        {
            path[i] = new Vector2I(Mathf.RoundToInt(_currentPath[i].X), Mathf.RoundToInt(_currentPath[i].Y));
        }
        return path;
    }

    //properties

    public Vector2[] currentPath
    {
        get { return _currentPath.ToArray(); }
    }
}
