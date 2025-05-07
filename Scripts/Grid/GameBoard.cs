using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan; YT: Heal Moon]
 * Last Updated: [05/07/2025]
 * [game board manages everything on the map]
 */

public partial class GameBoard : Node2D
{
    [Export] private GridResource _grid;
    [Export] private UnitWalkHighlight _unitWalkHighlights;
    [Export] private UnitPath _unitPath;
    [Export] private UnitManager _unitManager;
    [Export] private GridCursor _gridCursor;
    [Export] private Map _map;

    private Unit _selectedUnit;
    private Vector2[] _walkableCells;
    private float[,] _movementCosts;

    //all units, might want to split this up
    private Godot.Collections.Dictionary<Vector2, Unit> _units = new Godot.Collections.Dictionary<Vector2, Unit>();

    private const float MAX_VALUE = 9999999;

    /// <summary>
    /// reinitializes the units
    /// </summary>
    public override void _Ready()
    {
        _gridCursor.AcceptPress += OnCursorAcceptPress;
        _gridCursor.Moved += OnCursorMoved;

        _movementCosts = _map.GetMovementCosts(_grid);

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

        DeselectSelectedUnit();
        _selectedUnit.unitPathMovement.SetWalkPath(_unitPath.currentPath, _grid);

        await ToSignal(_selectedUnit.unitPathMovement, "WalkFinished");
        ClearSelectedUnit();
    }

    /// <summary>
    /// changes where the gameboard is tracking the location of units
    /// </summary>
    /// <param name="unit">unit to move</param>
    /// <param name="newLoc">new location</param>
    public void ChangeUnitLocationData(Unit unit, Vector2 newLoc)
    {
        if (!_units.ContainsKey(unit.cell))
        {
            return;
        }

        _units.Remove(unit.cell);
        _units[newLoc] = unit;
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
        return Dijksta(unit.cell, unit.unitStats.moveRange);
    }

    /// <summary>
    /// gets walkable units with move cost
    /// NOTE: tutorial said this is quick and dirty
    /// </summary>
    /// <param name="cell">unit coords</param>
    /// <param name="maxDistance">unit move stat</param>
    /// <returns>array of walkable units</returns>
    private Vector2[] Dijksta(Vector2 cell, float maxDistance)
    {
        int y = Mathf.RoundToInt(grid.gridCell.Y);
        int x = Mathf.RoundToInt(grid.gridCell.X);

        List<Vector2> moveableCells = new List<Vector2>();
        bool[,] visited = new bool[y,x];
        float[,] distances = new float[y, x];
        Vector2[,] previous = new Vector2[y, x];

        moveableCells.Add(cell);

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                visited[i, j] = false;
                distances[i, j] = MAX_VALUE;
            }
        }

        PriorityQueue<Vector2, float> dijkstaQueue = new PriorityQueue<Vector2, float>();
        dijkstaQueue.Enqueue(cell, 0);
        distances[Mathf.RoundToInt(cell.Y), Mathf.RoundToInt(cell.X)] = 0;

        float tileCost;
        float distanceToNode;

        while (dijkstaQueue.Count > 0)
        {
            dijkstaQueue.TryDequeue(out Vector2 currentValue, out float currentPriority);
            visited[Mathf.RoundToInt(cell.Y), Mathf.RoundToInt(cell.X)] = true;

            foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
            {
                Vector2 coords = currentValue + DirectionManager.Instance.GetVectorDirection(dir);
                int coordsY = Mathf.RoundToInt(coords.Y);
                int coordsX = Mathf.RoundToInt(coords.X);
                if (_grid.IsWithinBounds(coords))
                {
                    if (visited[coordsY, coordsX])
                    {
                        continue;
                    }
                    else
                    {
                        tileCost = _movementCosts[coordsY, coordsX];
                        distanceToNode = currentPriority + tileCost;

                        //checks if units can pass eachother
                        if (IsOccupied(coords) && !_unitManager.CanPass(_selectedUnit.unitGroup, _units[coords].unitGroup))
                        {
                            distanceToNode = currentPriority + MAX_VALUE;
                        }

                        visited[coordsY, coordsX] = true;
                        distances[coordsY, coordsX] = distanceToNode;
                    }

                    if (distanceToNode <= maxDistance)
                    {
                        previous[coordsY, coordsX] = currentValue;
                        moveableCells.Add(coords);
                        dijkstaQueue.Enqueue(coords, distanceToNode);
                    }
                }
            }
        }

        return moveableCells.ToArray();
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
