using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/04/2025]
 * [resouce for each grid map]
 */
[GlobalClass]
public partial class Grid : Resource
{
    [Export] private Vector2 _gridSize = new Vector2(20, 20);
    [Export] private Vector2 _cellSize = new Vector2(80, 80);
    private Vector2 _halfCellSize = Vector2.Zero;

    /// <summary>
    /// sets _halfCellSize since resources doesn't have a ready
    /// and C# cant reference other non static fields
    /// </summary>
    private void SetHalfCellSize()
    {
        if (_halfCellSize == Vector2.Zero)
        {
            _halfCellSize = _cellSize / 2;
        }
    }

    /// <summary>
    /// returns the position of a cell's center in pixel
    /// </summary>
    /// <param name="gridPosition">cell coordinates</param>
    /// <returns>position</returns>
    public Vector2 CalculateMapPosition(Vector2I gridPosition)
    {
        SetHalfCellSize();
        return (Vector2)gridPosition * _cellSize + _halfCellSize;
    }

    /// <summary>
    /// Returns the coordinates of the cell on the grid given a position on the map
    /// NOTE: you'll place units visually in the editor. We'll use this function to find
    /// the grid coordinates they're placed on, and call `calculate_map_position()` to snap them to the
    /// cell's center
    /// </summary>
    /// <param name="mapPosition">position on the map</param>
    /// <returns>coordinates of closest cell</returns>
    public Vector2 CalculateGridCoordinates(Vector2 mapPosition)
    {
        return (mapPosition / _cellSize).Floor();
    }

    /// <summary>
    /// returns if the coordinates are within the grid
    /// </summary>
    /// <param name="cellCoordinates">coordinates</param>
    /// <returns>true if the coordinates is within the grid</returns>
    public bool IsWithinBounds(Vector2 cellCoordinates)
    {
        return cellCoordinates.X >= 0 && cellCoordinates.X < _gridSize.X
            && cellCoordinates.Y >= 0 && cellCoordinates.Y < _gridSize.Y;
    }

    /// <summary>
    /// Makes the `grid_position` fit within the grid's bounds.
    /// This is a clamp function designed specifically for our grid coordinates.
    /// </summary>
    /// <param name="gridPosition">position</param>
    /// <returns>grid pos</returns>
    public Vector2 Clamp(Vector2 gridPosition)
    {
        Vector2 clampedOutput = gridPosition;
        clampedOutput.X = Mathf.Clamp(clampedOutput.X, 0, _gridSize.X - 1f);
        clampedOutput.Y = Mathf.Clamp(clampedOutput.Y, 0, _gridSize.Y - 1f);
        return clampedOutput;
    }

    /// <summary>
    /// Given Vector2 coordinates, calculates and returns the corresponding integer index
    /// </summary>
    /// <param name="cell">cell</param>
    /// <returns>index of cell</returns>
    public int AsIndex(Vector2 cell)
    {
        return (int)(cell.X + _gridSize.X * cell.Y);
    }
}
