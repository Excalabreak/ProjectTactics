using Godot;
using Godot.Collections;
using System;

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
    private Dictionary<Vector2, Node2D> _units = new Dictionary<Vector2, Node2D>();

    public override void _Ready()
    {
        Reinitialize();
        GD.Print(_units);
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
