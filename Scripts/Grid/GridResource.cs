using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [06/27/2025]
 * [holds information about a map]
 */

[GlobalClass]
public partial class GridResource : Resource
{
    [Export] private Vector2 _gridSize = new Vector2(20, 20);
    [Export] private Vector2 _cellSize = new Vector2(64, 64);

    private Vector2 _halfCell = Vector2.Zero;

    /// <summary>
    /// Calculates where to put a unit in the grid
    /// </summary>
    /// <param name="gridPos">coordinates of cell</param>
    /// <returns>position of grid</returns>
    public Vector2 CalculateMapPosition(Vector2 gridPos)
    {
        if (_halfCell == Vector2.Zero)
        {
            _halfCell = _cellSize / 2;
        }
        return gridPos * _cellSize + _halfCell;
    }

    /// <summary>
    /// Returns the coordinates of the cell on the grid given a position on the map 
    /// </summary>
    /// <param name="mapPos">position on map</param>
    /// <returns>Coordinates of cell</returns>
    public Vector2 CalculateGridCoordinates(Vector2 mapPos)
    {
        return (mapPos / _cellSize).Floor();
    }

    /// <summary>
    /// returns true if cellCoordinates is within the map
    /// </summary>
    /// <param name="cellCoordinates">coordinate of cell</param>
    /// <returns>true if cellCoordinates is within the map</returns>
    public bool IsWithinBounds(Vector2 cellCoordinates)
    {
        return cellCoordinates.X >= 0 && cellCoordinates.X < _gridSize.X
            && cellCoordinates.Y >= 0 && cellCoordinates.Y < _gridSize.Y;
    }

    /// <summary>
    /// clamps the gridPos to make sure it's in the grid
    /// </summary>
    /// <param name="gridPos">coordinate of cell</param>
    /// <returns>clamped coordinates</returns>
    public Vector2 Clamp(Vector2 gridPos)
    {
        Vector2 output = gridPos;
        output.X = Mathf.Clamp(output.X, 0, _gridSize.X - 1f);
        output.Y = Mathf.Clamp(output.Y, 0, _gridSize.Y - 1f);
        return output;
    }

    /// <summary>
    /// Given Vector2 coordinates, calculates and returns the corresponding integer index.
    /// convert 2D coordinates to a 1D array's indices.
    /// 
    /// for the AStar algorithm
    /// which requires a unique index for each point on the graph it uses to find a path.
    /// </summary>
    /// <param name="cell">coordinate of cell</param>
    /// <returns>index of cell</returns>
    public int AsIndex(Vector2 cell)
    {
        return (int)(cell.X + _gridSize.X * cell.Y);
    }

    public Vector2[] GetAllCellCoords()
    {
        int x = Mathf.RoundToInt(_gridSize.X);
        int y = Mathf.RoundToInt(_gridSize.Y);

        Vector2[] coords = new Vector2[(x * y)];

        int index = 0;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                coords[index] = new Vector2(i, j);
                index++;
            }
        }
        return coords;
    }

    public Vector2 gridCell
    {
        get { return _gridSize; }
    }

    public Vector2 gridSize
    {
        get { return _gridSize; }
    }
}
