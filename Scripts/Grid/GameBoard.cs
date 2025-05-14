using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: 
 * 
 * Lovato, Nathan: map
 * YT: Heal Moon: unit movement
 * YT: DAY 345: Line of sight
 * YT: NoBS Code: circle
 * 
 * Last Updated: [05/13/2025]
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
    [Export] private FogOfWar _fogOfWar;
    [Export] private BlockedOverlay _blockedOverlay;

    private Unit _selectedUnit;
    private Vector2[] _walkableCells;
    private float[,] _movementCosts;

    //all units, might want to split this up
    private System.Collections.Generic.Dictionary<Vector2, Unit> _units = new System.Collections.Generic.Dictionary<Vector2, Unit>();

    //probably have to redo this
    private System.Collections.Generic.Dictionary<Unit, Vector2[]> _unitVision = new System.Collections.Generic.Dictionary<Unit, Vector2[]>();
    private System.Collections.Generic.Dictionary<Vector2, List<Unit>> _cellRevealedBy = new System.Collections.Generic.Dictionary<Vector2, List<Unit>>();


    private const float MAX_VALUE = 9999999;

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
    /// gets all the units in the map
    /// </summary>
    private void Reinitialize()
    {
        _units.Clear();
        _fogOfWar.HideWholeMap();

        foreach (Unit unit in _unitManager.GetAllUnits())
        {
            _units[unit.cell] = unit;
            UpdateUnitVision(unit);
        }

        _movementCosts = _map.GetMovementCosts(_grid);


        
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

    /// <summary>
    /// clears unit from selected
    /// </summary>
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

    /*
    /// <summary>
    /// updates the PLAYER unit vision on game map
    /// AFTER THE UNIT HAS MOVED
    /// </summary>
    /// <param name="unit">unit being updated</param>
    public void UpdateUnitVision(Unit unit)
    {
        //returns on non player units since only the player can see their units
        if (unit.unitGroup != UnitGroupEnum.PLAYER)
        {
            return;
        }

        //hides unit's old vision
        //adds unit to vision dictionary if not there
        HideVision(unit, true);

        //Vector2I for which tiles get checked
        Vector2I startingCell = new Vector2I(Mathf.RoundToInt(unit.cell.X), Mathf.RoundToInt(unit.cell.Y));
        Vector2I unitFacing = DirectionManager.Instance.GetVectorIDirection(unit.unitDirection.currentFacing);
        Vector2I visionExpand = Vector2I.Down;

        if (unit.unitDirection.currentFacing == DirectionEnum.UP ||
            unit.unitDirection.currentFacing == DirectionEnum.DOWN)
        {
            visionExpand = Vector2I.Right;
        }

        //list of tiles
        List<Vector2I> visibleTiles = new List<Vector2I>();
        List<Vector2I> checkTiles = new List<Vector2I>();
        List<Vector2I> addedTiles = new List<Vector2I>();

        //adds the fist check
        visibleTiles.Add(startingCell);
        if (_grid.IsWithinBounds(startingCell + unitFacing))
        {
            checkTiles.Add(startingCell + unitFacing);
        }
        if (_grid.IsWithinBounds(startingCell + unitFacing + visionExpand))
        {
            checkTiles.Add(startingCell + unitFacing + visionExpand);
        }
        if (_grid.IsWithinBounds(startingCell + unitFacing + -visionExpand))
        {
            checkTiles.Add(startingCell + unitFacing + -visionExpand);
        }

        //current vars
        int checkDistance = 1;
        bool hasAdded = true;

        //loop that checks tiles
        while (hasAdded && checkTiles.Count > 0)
        {
            hasAdded = false;
            addedTiles.Clear();

            //checks line from unit to current checking tile
            foreach (Vector2I checkCoords in checkTiles)
            {
                List<Vector2I> tileLine = new List<Vector2I>();

                int dx = Math.Abs(checkCoords.X - startingCell.X);
                int dy = Math.Abs(checkCoords.Y - startingCell.Y);
                int sx = startingCell.X < checkCoords.X ? 1 : -1;
                int sy = startingCell.Y < checkCoords.Y ? 1 : -1;
                int err = dx - dy;

                Vector2I current = startingCell;

                while (true)
                {
                    tileLine.Add(current);
                    //breaks if tile is within range
                    if (current == checkCoords)
                    {
                        tileLine.RemoveAt(0);
                        float unitVisionCost = 0f;

                        foreach (Vector2I tile in tileLine)
                        {
                            if (IsOccupied(tile) && !_unitManager.CanPass(unit.unitGroup, _units[tile].unitGroup))
                            {
                                unitVisionCost += 2f;
                            }
                        }

                        if (unit.unitStats.visionRange < 
                           (_map.GetTilePathVisionCost(tileLine) * (startingCell.DistanceTo(checkCoords)
                           / checkDistance))+ unitVisionCost)
                        {
                            //has to check if something is blocking is what is eating up the rest of the walk cost
                            break;
                        }
                        visibleTiles.Add(checkCoords);
                        addedTiles.Add(checkCoords);
                        hasAdded = true;
                        break;
                    }

                    int e2 = 2 * err;
                    if (e2 > -dy)
                    {
                        err -= dy;
                        current.X += sx;
                    }
                    if (e2 < dx)
                    {
                        err += dx;
                        current.Y += sy;
                    }
                }
            }
            checkTiles.Clear();

            //adds tiles to be checked
            if (hasAdded)
            {
                checkDistance++;
                
                foreach (Vector2I addedCoord in addedTiles)
                {
                    Vector2I nextCoord = addedCoord + unitFacing;
                    if (!checkTiles.Contains(nextCoord) && _grid.IsWithinBounds(nextCoord + visionExpand))
                    {
                        checkTiles.Add(nextCoord);
                    }

                    if (unit.unitDirection.currentFacing == DirectionEnum.UP ||
                    unit.unitDirection.currentFacing == DirectionEnum.DOWN)
                    {
                        if (startingCell.X <= addedCoord.X && 
                            _grid.IsWithinBounds(nextCoord + visionExpand) &&
                            !checkTiles.Contains(nextCoord + visionExpand))
                        {
                            checkTiles.Add(nextCoord + visionExpand);
                        }
                        else if (startingCell.X >= addedCoord.X && 
                            _grid.IsWithinBounds(nextCoord + -visionExpand) &&
                            !checkTiles.Contains(nextCoord + -visionExpand))
                        {
                            checkTiles.Add(nextCoord + -visionExpand);
                        }
                    }
                    else
                    {
                        if (startingCell.Y <= addedCoord.Y && 
                            _grid.IsWithinBounds(nextCoord + visionExpand) &&
                            !checkTiles.Contains(nextCoord + visionExpand))
                        {
                            checkTiles.Add(nextCoord + visionExpand);
                        }
                        else if (startingCell.Y >= addedCoord.Y && 
                            _grid.IsWithinBounds(nextCoord + -visionExpand) &&
                            !checkTiles.Contains(nextCoord + -visionExpand))
                        {
                            checkTiles.Add(nextCoord + -visionExpand);
                        }
                    }
                }
            }
        }

        //visible tiles get revealed
        Vector2[] output = new Vector2[visibleTiles.Count()];
        for (int i = 0; i < visibleTiles.Count(); i++)
        {
            output[i] = visibleTiles[i];
        }
        ShowVision(unit, output);
    }*/

    /// <summary>
    /// updates the PLAYER unit vision on game map
    /// AFTER THE UNIT HAS MOVED
    /// </summary>
    /// <param name="unit">unit being updated</param>
    public void UpdateUnitVision(Unit unit)
    {
        //returns on non player units since only the player can see their units
        if (unit.unitGroup != UnitGroupEnum.PLAYER)
        {
            return;
        }

        //hides unit's old vision
        //adds unit to vision dictionary if not there
        HideVision(unit, true);

        //Vector2I for which tiles get checked
        Vector2I startingCell = new Vector2I(Mathf.RoundToInt(unit.cell.X), Mathf.RoundToInt(unit.cell.Y));
        Vector2I unitFacing = DirectionManager.Instance.GetVectorIDirection(unit.unitDirection.currentFacing);
        Vector2I visionExpand = Vector2I.Down;

        if (unit.unitDirection.currentFacing == DirectionEnum.UP ||
            unit.unitDirection.currentFacing == DirectionEnum.DOWN)
        {
            visionExpand = Vector2I.Right;
        }

        //list of tiles
        List<Vector2I> visibleTiles = new List<Vector2I>();
        List<Vector2I> checkTiles = new List<Vector2I>();

        visibleTiles.Add(startingCell);

        int r = Mathf.RoundToInt(unit.unitStats.visionRange);

        int count = r-3;
        for (int i = 0; i < count; i++)
        {
            int x = 0;
            int y = -r;
            int p = -r;

            while (x < -y)
            {
                if (p > 0)
                {
                    y += 1;
                    p += 2 * (x + y) + 1;
                }
                else
                {
                    p += 2 * x + 1;
                }

                switch (unit.unitDirection.currentFacing)
                {
                    case DirectionEnum.UP:
                        checkTiles.Add(startingCell + new Vector2I(x, y));
                        checkTiles.Add(startingCell + new Vector2I(-x, y));
                        break;
                    case DirectionEnum.DOWN:
                        checkTiles.Add(startingCell + new Vector2I(x, -y));
                        checkTiles.Add(startingCell + new Vector2I(-x, -y));
                        break;
                    case DirectionEnum.RIGHT:
                        checkTiles.Add(startingCell + new Vector2I(-y, x));
                        checkTiles.Add(startingCell + new Vector2I(-y, -x));
                        break;
                    case DirectionEnum.LEFT:
                        checkTiles.Add(startingCell + new Vector2I(y, x));
                        checkTiles.Add(startingCell + new Vector2I(y, -x));
                        break;
                    default:
                        GD.Print("something went wrong");
                        break;
                }

                x += 1;
            }
            r--;
        }
        

        //checks line from unit to current checking tile
        foreach (Vector2I checkCoords in checkTiles)
        {
            List<Vector2I> tileLine = new List<Vector2I>();

            int dx = Math.Abs(checkCoords.X - startingCell.X);
            int dy = Math.Abs(checkCoords.Y - startingCell.Y);
            int sx = startingCell.X < checkCoords.X ? 1 : -1;
            int sy = startingCell.Y < checkCoords.Y ? 1 : -1;
            int err = dx - dy;

            Vector2I current = startingCell;

            while (true)
            {
                tileLine.Add(current);
                //breaks if tile is within range
                if (current == checkCoords)
                {
                    tileLine.RemoveAt(0);

                    float visionCost = 0f;
                    for (int i = 0; i < tileLine.Count; i++)
                    {
                        if (!_grid.IsWithinBounds(tileLine[i]))
                        {
                            break;
                        }

                        float curVisionCost = 0f;

                        curVisionCost += _map.GetTileVisionCost(tileLine[i]);

                        if (IsOccupied(tileLine[i]) && !_unitManager.CanPass(unit.unitGroup, _units[tileLine[i]].unitGroup))
                        {
                            curVisionCost += 2f;
                        }

                        if (unit.unitStats.visionRange < visionCost + curVisionCost)
                        {
                            if (unit.unitStats.visionRange < visionCost)
                            {
                                visibleTiles.Add(tileLine[i]);
                            }
                            break;
                        }

                        visibleTiles.Add(tileLine[i]);
                        visionCost += curVisionCost;
                    }
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    current.X += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    current.Y += sy;
                }
            }
        }

        //visible tiles get revealed
        Vector2[] output = new Vector2[visibleTiles.Count()];
        for (int i = 0; i < visibleTiles.Count(); i++)
        {
            output[i] = visibleTiles[i];
        }
        ShowVision(unit, output);
    }

    /// <summary>
    /// Hides vision of unit
    /// </summary>
    /// <param name="unit">unit being updated</param>
    public void HideVision(Unit unit, bool keepUnitKey)
    {
        //hides unit's old vision
        if (_unitVision.ContainsKey(unit))
        {
            foreach (Vector2 coords in _unitVision[unit])
            {
                _cellRevealedBy[coords].Remove(unit);
                if (_cellRevealedBy[coords].Count <= 0)
                {
                    _fogOfWar.HideMapCell(coords);
                    _cellRevealedBy.Remove(coords);
                }
            }
            if (!keepUnitKey)
            {
                _unitVision.Remove(unit);
            }
        }
    }

    /// <summary>
    /// reveals fog of war maps for units
    /// updates data structures
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="coords"></param>
    public void ShowVision(Unit unit, Vector2[] coords)
    {
        if (!_unitVision.ContainsKey(unit))
        {
            _unitVision.Add(unit, coords);
        }
        else
        {
            _unitVision[unit] = coords;
        }

        foreach (Vector2 coord in coords)
        {
            if (!_cellRevealedBy.ContainsKey(coord))
            {
                _cellRevealedBy.Add(coord, new List<Unit>());
            }
            _cellRevealedBy[coord].Add(unit);

            _fogOfWar.RevealMapCell(coord);
        }
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

    //properties
    //note, make export if need to test outside of game board
    public GridResource grid
    {
        get { return _grid; }
    }
}
