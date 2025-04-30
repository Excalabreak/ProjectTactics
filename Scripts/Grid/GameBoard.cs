using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [04/28/2025]
 * [game board manages everything on the map]
 */

public partial class GameBoard : Node2D
{
    [Export] private GridResource _grid;
    [Export] private UnitWalkHighlight _unitWalkHighlights;
    [Export] private UnitPath _unitPath;
    [Export] private UnitManager _unitManager;
    [Export] private GridCursor _gridCursor;

    private Unit _selectedUnit;
    private Vector2[] _walkableCells;

    //all units, might want to split this up
    private Godot.Collections.Dictionary<Vector2, Unit> _units = new Godot.Collections.Dictionary<Vector2, Unit>();

    /// <summary>
    /// reinitializes the units
    /// </summary>
    public override void _Ready()
    {
        _gridCursor.AcceptPress += OnCursorAcceptPress;
        _gridCursor.Moved += OnCursorMoved;

        Reinitialize();
    }

    /// <summary>
    /// if the player inputs decline, deselect unit
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (_selectedUnit != null && @event.IsActionPressed("Decline"))
        {
            DeselectSelectedUnit();
            ClearSelectedUnit();
        }
    }

    /// <summary>
    /// selects unit
    /// </summary>
    /// <param name="cell">cell to select unit</param>
    private void SelectUnit(Vector2 cell)
    {
        if (!_units.ContainsKey(cell))
        {
            //could add options here
            return;
        }

        _selectedUnit = _units[cell];
        _selectedUnit.isSelected = true;
        _walkableCells = GetWalkableCells(_selectedUnit);
        _unitWalkHighlights.DrawHighlights(_walkableCells);
        _unitPath.Initialize(_walkableCells);
    }

    /// <summary>
    /// deselects units and clears overlays
    /// </summary>
    private void DeselectSelectedUnit()
    {
        _selectedUnit.isSelected = false;
        _unitWalkHighlights.Clear();
        _unitPath.Stop();
    }

    private void ClearSelectedUnit()
    {
        _selectedUnit = null;
        _walkableCells = new Vector2[0];
    }

    /// <summary>
    /// moves the selected unit to a new cell
    /// </summary>
    /// <param name="newCell"></param>
    private async void MoveSelectedUnit(Vector2 newCell)
    {
        if (IsOccupied(newCell) || !_walkableCells.Contains(newCell))
        {
            return;
        }

        _units.Remove(_selectedUnit.cell);
        _units[newCell] = _selectedUnit;
        DeselectSelectedUnit();
        _selectedUnit.unitPathMovement.SetWalkPath(_unitPath.currentPath, _grid);

        await ToSignal(_selectedUnit.unitPathMovement, "WalkFinished");
        ClearSelectedUnit();
    }

    /// <summary>
    /// updates the unit path when the cursor moves
    /// </summary>
    /// <param name="newCell">cell cursor is moving to</param>
    private void OnCursorMoved(Vector2 newCell)
    {
        if (_selectedUnit != null && _selectedUnit.isSelected)
        {
            _unitPath.DrawPath(_selectedUnit.cell, newCell);
        }
    }

    /// <summary>
    /// calls to select or move unit
    /// </summary>
    /// <param name="cell">cell cursor is on</param>
    private void OnCursorAcceptPress(Vector2 cell)
    {
        if (_selectedUnit == null)
        {
            SelectUnit(cell);
        }
        else if (_selectedUnit.isSelected)
        {
            MoveSelectedUnit(cell);
        }
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
    /// makes sure signals are unsubscribed
    /// </summary>
    public override void _ExitTree()
    {
        _gridCursor.AcceptPress -= OnCursorAcceptPress;
        _gridCursor.Moved -= OnCursorMoved;
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

        foreach (Unit unit in _unitManager.GetAllUnits())
        {
            _units[unit.cell] = unit;
        }
    }

    //properties
    //note, make export if need to test outside of game board
    public GridResource grid
    {
        get { return _grid; }
    }
}
