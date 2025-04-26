using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [04/26/2025]
 * [game board manages everything on the map]
 */

public partial class GameBoard : Node2D
{
    [Export] private GridResource _grid;

    //all units, might want to split this up
    private Godot.Collections.Dictionary<Vector2, Unit> _units = new Godot.Collections.Dictionary<Vector2, Unit>();

    /// <summary>
    /// reinitializes the units
    /// </summary>
    public override void _Ready()
    {
        Reinitialize();

    }

    /// <summary>
    /// Returns an array of cells a given unit can walk using the flood fill algorithm
    /// </summary>
    /// <param name="unit">selected unit</param>
    /// <returns>array of coords that the unit can walk</returns>
    private Vector2[] GetWalkableCells(Unit unit)
    {
        return FloodFill(unit.cell, unit.unitStats.moveRange);
    }

    /// <summary>
    /// Returns an array with all the coordinates of walkable cells based on the `max_distance`
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="maxDistance">how far the unit can walk</param>
    /// <returns>array of coords that the unit can walk</returns>
    private Vector2[] FloodFill(Vector2 cell, int maxDistance)
    {
        List<Vector2> walkableCells = new List<Vector2>();

        Stack<Vector2> checkingCells = new Stack<Vector2>();
        checkingCells.Push(cell);

        while (checkingCells.Count > 0)
        {
            Vector2 current = checkingCells.Pop();

            if (!_grid.IsWithinBounds(current) || walkableCells.Contains(current))
            {
                continue;
            }

            Vector2 difference = (current - cell).Abs();
            int distance = (int)Mathf.Round(difference.X + difference.Y);
            if (distance > maxDistance)
            {
                continue;
            }

            walkableCells.Add(current);
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                Vector2 nextCoords = current + VectorDirections.Instance.GetDirection(dir);

                if (IsOccupied(nextCoords) || walkableCells.Contains(nextCoords))
                {
                    continue;
                }

                checkingCells.Push(nextCoords);
            }
        }
        return walkableCells.ToArray();
    }

    /// <summary>
    /// Returns `true` if the cell is occupied by a unit.
    /// </summary>
    /// <param name="cell">coordinates of grid that are occupied</param>
    /// <returns>true if cell is occupied</returns>
    private bool IsOccupied(Vector2 cell)
    {
        return _units.ContainsKey(cell);
    }

    /// <summary>
    /// gets all the units in the map
    /// </summary>
    private void Reinitialize()
    {
        _units.Clear();

        //temp, we will need to change this for a better system
        foreach (Node2D child in GetChildren())
        {
            var unit = child as Unit;
            if (unit == null)
            {
                continue;
            }
            _units[unit.cell] = unit;
        }
    }
}
