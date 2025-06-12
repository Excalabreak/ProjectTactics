using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: 
 * 
 * Lovato, Nathan: map
 * YT:
 * Heal Moon: unit movement, attack range, menu
 * DAY 345: Line of sight
 * NoBS Code: Circle and Xiaolin Wu Line Algorithm
 * 
 * Last Updated: [06/11/2025]
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

    [Export] private MenuStateMachine _menuStateMachine;
    [Export] private PackedScene _actionMenu;
    [Export] private PackedScene _pauseMenu;
    [Signal] public delegate void SelectedMovedEventHandler();

    private Unit _selectedUnit;
    private Vector2[] _walkableCells;
    private Vector2[] _attackableCells;
    private float[,] _movementCosts;

    private ActionMenu _actionMenuInstance;
    private PauseScreen _pauseScreenInstance;

    //all units, might want to split this up
    private System.Collections.Generic.Dictionary<Vector2, Unit> _units = new System.Collections.Generic.Dictionary<Vector2, Unit>();

    //Dictionary for the fog of war
    private System.Collections.Generic.Dictionary<Unit, Vector2[]> _unitVision = new System.Collections.Generic.Dictionary<Unit, Vector2[]>();
    private System.Collections.Generic.Dictionary<Vector2, List<Unit>> _cellRevealedBy = new System.Collections.Generic.Dictionary<Vector2, List<Unit>>();
    private System.Collections.Generic.Dictionary<Unit, Vector2[]> _unitVisionBlocked = new System.Collections.Generic.Dictionary<Unit, Vector2[]>();
    private System.Collections.Generic.Dictionary<Vector2, List<Unit>> _cellBlockedBy = new System.Collections.Generic.Dictionary<Vector2, List<Unit>>();

    private const float MAX_VALUE = 9999999;

    //---------- SET UP -----------

    /// <summary>
    /// reinitializes the units
    /// </summary>
    public override void _Ready()
    {
        _gridCursor.AcceptPress += OnCursorAcceptPress;
        _gridCursor.Moved += OnCursorMoved;
        _gridCursor.Decline += OnCursorDecline;

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

        foreach (KeyValuePair<Vector2,Unit> unit in _units)
        {
            UpdateUnitVision(unit.Value);
        }

        _movementCosts = _map.GetMovementCosts(_grid);
    }

    /// <summary>
    /// makes sure signals are unsubscribed
    /// </summary>
    public override void _ExitTree()
    {
        _gridCursor.AcceptPress -= OnCursorAcceptPress;
        _gridCursor.Moved -= OnCursorMoved;
        _gridCursor.Decline -= OnCursorDecline;
    }

    //------------ INPUT ----------

    /// <summary>
    /// calls state machine to handle which cursor move
    /// method to use
    /// </summary>
    /// <param name="newCell">cell cursor is moving to</param>
    private void OnCursorMoved(Vector2 newCell)
    {
        _menuStateMachine.currentState.OnCursorMove(newCell);
    }

    /// <summary>
    /// calls state machine to handle which accept press
    /// method to use
    /// </summary>
    /// <param name="cell">cell cursor is on</param>
    private void OnCursorAcceptPress(Vector2 cell)
    {
        _menuStateMachine.currentState.OnCursorAccept(cell);
    }

    private void OnCursorDecline()
    {
        _menuStateMachine.currentState.OnCursorDecline();
    }

    //------------ SELECT UNIT ----------

    /// <summary>
    /// selects unit
    /// </summary>
    /// <param name="cell">cell to select unit</param>
    private void SelectUnit(Vector2 cell)
    {
        if (!_units.ContainsKey(cell))
        {
            return;
        }

        _selectedUnit = _units[cell];
        _selectedUnit.isSelected = true;

        _walkableCells = GetWalkableCells(_selectedUnit);
        _attackableCells = GetAttackableCells(_selectedUnit);

        _unitWalkHighlights.DrawAttackHighlights(_attackableCells);
        _unitWalkHighlights.DrawWalkHighlights(_walkableCells);

        _unitPath.Initialize(_walkableCells, _selectedUnit.cell);
    }

    /// <summary>
    /// displays information when the cursor is hovering a unit
    /// </summary>
    /// <param name="cell">cell to display info</param>
    public void HoverDisplay(Vector2 cell)
    {
        //add condition for hidden enemy unit
        if (!_units.ContainsKey(cell))
        {
            //could add options here
            return;
        }

        Unit curUnit = _units[cell];

        _walkableCells = GetWalkableCells(curUnit);
        _attackableCells = GetAttackableCells(curUnit);

        _unitWalkHighlights.DrawAttackHighlights(_attackableCells);
        _unitWalkHighlights.DrawWalkHighlights(_walkableCells);
    }

    /// <summary>
    /// deselects units and clears overlays
    /// </summary>
    public void DeselectSelectedUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.isSelected = false;
        }
        _unitWalkHighlights.Clear();
        _unitPath.Stop();
    }

    /// <summary>
    /// clears unit from selected
    /// </summary>
    public void ClearSelectedUnit()
    {
        _selectedUnit = null;
        _walkableCells = new Vector2[0];
    }

    //---------- MOVE UNIT -----------

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
        EmitSignal("SelectedMoved");
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

    //---------- MENU CURSOR ACCEPT -----------

    /// <summary>
    /// selects unit and brings up menu if needed
    /// </summary>
    /// <param name="cell"></param>
    public void MenuUnSelectedStateAccept(Vector2 cell)
    {
        //need to add a condition for enemy units
        if (_selectedUnit == null && _units.ContainsKey(cell))
        {
            SelectUnit(cell);
            _actionMenuInstance = _actionMenu.Instantiate() as ActionMenu;
            AddChild(_actionMenuInstance);
        }
        else if (_selectedUnit != null)
        {
            return;
        }
        else if (_selectedUnit == null)
        {
            _pauseScreenInstance = _pauseMenu.Instantiate() as PauseScreen;
            AddChild(_pauseScreenInstance);
        }
    }

    /// <summary>
    /// moves unit to cell
    /// </summary>
    /// <param name="cell"></param>
    public async void MenuMoveStateAccept(Vector2 cell)
    {
        if (!_walkableCells.Contains(cell))
        {
            return;
        }

        if (IsOccupied(cell) && _units[cell] == _selectedUnit)
        {
            _units.Remove(_selectedUnit.cell);
            _units[cell] = _selectedUnit;
            DeselectSelectedUnit();
            ClearSelectedUnit();
            _walkableCells = new Vector2[0];
            _unitWalkHighlights.Clear();
        }
        else if (!IsOccupied(cell) && _walkableCells.Contains(cell))
        {
            //wait for unit to move
            MoveSelectedUnit(cell);
            _walkableCells = new Vector2[0];
            _unitWalkHighlights.Clear();
            await ToSignal(this, "SelectedMoved");
        }
        _menuStateMachine.TransitionTo("UnSelectedState");
    }

    /// <summary>
    /// selects unit for attack
    /// </summary>
    /// <param name="cell"></param>
    public void MenuAttackStateAccept(Vector2 cell)
    {
        if (!IsOccupied(cell))
        {
            return;
        }

        Unit opposingUnit = _units[cell];
        if (!_unitManager.CanAttack(_selectedUnit.unitGroup, opposingUnit.unitGroup))
        {
            return;
        }

        Vector2[] attackableCells = FloodFill(_selectedUnit.cell, _selectedUnit.attackRange);
        if (!attackableCells.Contains(cell))
        {
            return;
        }

        //very basic combat for now
        _unitWalkHighlights.Clear();

        UnitStats attackingStats = _selectedUnit.unitStats;
        UnitStats defendingStats = opposingUnit.unitStats;
        Battle(attackingStats, defendingStats);

        //counter attack
        //temp, there is a faster way of doing this
        //figure out later
        Vector2[] opposingAttackableCells = FloodFill(opposingUnit.cell, opposingUnit.attackRange);
        if (opposingAttackableCells.Contains(_selectedUnit.cell))
        {
            Battle(defendingStats, attackingStats);
        }

        DeselectSelectedUnit();
        ClearSelectedUnit();
        _menuStateMachine.TransitionTo("UnSelectedState");
    }

    private void Battle(UnitStats attackingUnit, UnitStats defendingUnit)
    {
        GD.Print("uses base stat for battle");
        defendingUnit.DamageUnit(attackingUnit.GetBaseStat(UnitStatEnum.STRENGTH) - defendingUnit.GetBaseStat(UnitStatEnum.DEFENSE));
    }

    //---------- MENU CURSOR MOVE ----------

    /// <summary>
    /// shows the hover display for unit on new cell
    /// </summary>
    /// <param name="newCell">new cell</param>
    public void MenuUnSelectedStateCursorMove(Vector2 newCell)
    {
        _unitWalkHighlights.Clear();
        if (_units.ContainsKey(newCell) && _selectedUnit == null)
        {
            HoverDisplay(newCell);
        }
    }

    /// <summary>
    /// draws path for move action
    /// </summary>
    /// <param name="newCell"></param>
    public void MenuMoveStateCursorMove(Vector2 newCell)
    {
        //things to check for auto path:
        //is a walkable cell (WISH: maybe path find around if not walkable)
        //if coords even connects
        //path move cost
        if (!_walkableCells.Contains(newCell))
        {
            _unitPath.DrawAutoPath(_selectedUnit.cell, newCell);
            return;
        }
        if (!_unitPath.CoordConnects(newCell))
        {
            _unitPath.DrawAutoPath(_selectedUnit.cell, newCell);
            return;
        }

        GD.Print("uses base stat for move");

        Vector2I intNewCell = new Vector2I(Mathf.RoundToInt(newCell.X), Mathf.RoundToInt(newCell.Y));
        if (_map.GetPathMoveCost(_unitPath.GetIntCurrentPath()) + _map.GetTileMoveCost(intNewCell) 
            > _selectedUnit.unitStats.GetBaseStat(UnitStatEnum.MOVE))
        {
            _unitPath.DrawAutoPath(_selectedUnit.cell, newCell);
            return;
        }

        _unitPath.AddTileToCurrentPath(newCell);
    }

    //---------- MENU CURSOR DECLINE ----------

    /// <summary>
    /// default cursor decline for menu, 
    /// just changes state to unselected state
    /// </summary>
    public void StandardCursorDecline()
    {
        if (IsInstanceValid(_actionMenuInstance))
        {
            _actionMenuInstance.OnCancelButtonPress();
        }
        //might want to change so gameboard isn't handling options
        else if (IsInstanceValid(_pauseScreenInstance))
        {
            _pauseScreenInstance.OnClosePressed();
        }
        ResetMenu();
    }

    public void MoveStateCursorDecline()
    {
        if (_gridCursor.cell == _selectedUnit.cell)
        {
            StandardCursorDecline();
            return;
        }

        ResetMovePath();
        _unitPath.ResetCurrentPath();
    }

    //---------- OTHER MENU ----------

    /// <summary>
    /// resets menu
    /// </summary>
    public void ResetMenu()
    {
        DeselectSelectedUnit();
        ClearSelectedUnit();
        menuStateMachine.TransitionTo("UnSelectedState");
    }

    /// <summary>
    /// moves the grid cursor back onto the selected unit
    /// </summary>
    public void ResetMovePath()
    {
        if (_gridCursor.isMouse)
        {
            _gridCursor.WarpMouseWithoutSignal(_selectedUnit.GetGlobalTransformWithCanvas().Origin);
        }
        else
        {
            _gridCursor.cell = _selectedUnit.cell;
        }
    }

    //---------- DISPLAY HIGHLIGHTS ----------

    /// <summary>
    /// shows the attack range of the current selected unit
    /// </summary>
    public void ShowCurrentAttackRange()
    {
        _unitWalkHighlights.Clear();
        _unitWalkHighlights.DrawAttackHighlights(FloodFill(_selectedUnit.cell, _selectedUnit.attackRange));
    }

    /// <summary>
    /// Returns an array of cells a given unit can walk using the flood fill algorithm
    /// </summary>
    /// <param name="unit">selected unit</param>
    /// <returns>array of coords that the unit can walk</returns>
    private Vector2[] GetWalkableCells(Unit unit)
    {
        GD.Print("uses base stat for getting walkable cells");
        return Dijksta(unit.cell, (float)unit.unitStats.GetBaseStat(UnitStatEnum.MOVE), false);
    }

    /// <summary>
    /// gets the cells a unit can attack
    /// NOTE: inefficient, optimize later
    /// </summary>
    /// <param name="unit">selected unit</param>
    /// <returns>array of cell coordinates</returns>
    private Vector2[] GetAttackableCells(Unit unit)
    {
        GD.Print("uses base stat for getting attackable cell");
        List<Vector2> attackableCells = new List<Vector2>();
        Vector2[] realWalkableCells = Dijksta(unit.cell, unit.unitStats.GetBaseStat(UnitStatEnum.MOVE), true);

        foreach (Vector2 curCell in realWalkableCells)
        {
            for (int i = 1; i <= unit.attackRange + 1; i++)
            {
                attackableCells.AddRange(FloodFill(curCell, unit.attackRange));
            }
        }

        return attackableCells.Except(realWalkableCells).ToArray();
    }

    //----------- HIGHLIGHT ALGORITHIMS -----------

    /// <summary>
    /// flood fills based on cell coordinates and max distance
    /// </summary>
    /// <param name="cell">coords</param>
    /// <param name="maxDistance">distance of flood fill</param>
    /// <returns>array of coords</returns>
    private Vector2[] FloodFill(Vector2 cell, int maxDistance)
    {
        List<Vector2> output = new List<Vector2>();
        List<Vector2> walls = new List<Vector2>();
        Stack<Vector2> cellStack = new Stack<Vector2>();

        cellStack.Push(cell);

        while (cellStack.Count > 0)
        {
            Vector2 current = cellStack.Pop();

            if (!_grid.IsWithinBounds(current))
            {
                continue;
            }
            if (output.Contains(current))
            {
                continue;
            }

            Vector2 differences = (current - cell).Abs();
            int distances = Mathf.RoundToInt(differences.X) + Mathf.RoundToInt(differences.Y);
            if (distances > maxDistance)
            {
                continue;
            }

            output.Add(current);
            foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
            {
                Vector2 coords = current + DirectionManager.Instance.GetVectorDirection(dir);

                if (!_grid.IsWithinBounds(coords))
                {
                    continue;
                }

                if (_map.GetTileMoveCost(new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y))) > 99)
                {
                    walls.Add(coords);
                }

                /*
                if (IsOccupied(coords))
                {
                    continue;
                }*/
                if (output.Contains(coords))
                {
                    continue;
                }
                if (cellStack.Contains(coords))
                {
                    continue;
                }

                cellStack.Push(coords);
            }
        }


        return output.Except(walls).ToArray();
    }

    /// <summary>
    /// gets walkable units with move cost
    /// NOTE: tutorial said this is quick and dirty
    /// </summary>
    /// <param name="cell">unit coords</param>
    /// <param name="maxDistance">unit move stat</param>
    /// <param name="attackableCheck">check unit attack check</param>
    /// <returns>array of walkable units</returns>
    private Vector2[] Dijksta(Vector2 cell, float maxDistance, bool attackableCheck)
    {
        int y = Mathf.RoundToInt(grid.gridCell.Y);
        int x = Mathf.RoundToInt(grid.gridCell.X);

        Unit curUnit = _units[cell];
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
        List<Vector2> occupiedCells = new List<Vector2>();

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
                        if (IsOccupied(coords))
                        {
                            if (!_unitManager.CanPass(curUnit.unitGroup, _units[coords].unitGroup))
                            {
                                distanceToNode = currentPriority + MAX_VALUE;
                            }
                            else if (_units[coords].isWait && attackableCheck)
                            {
                                occupiedCells.Add(coords);
                            }
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

        return moveableCells.Except(occupiedCells).ToArray();
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

    //---------- VISION/FOG OF WAR ----------

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

        //list of tiles
        List<Vector2I> checkTiles = new List<Vector2I>();
        List<Vector2I> visibleTiles = new List<Vector2I>();
        List<Vector2I> blockingTiles = new List<Vector2I>();

        visibleTiles.Add(startingCell);

        //GD.Print("uses base stat for vision");
        int r = unit.unitStats.GetBaseStat(UnitStatEnum.VISION);

        int count = r/2;
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
            bool reverse = false;

            float x0 = startingCell.X;
            float y0 = startingCell.Y;
            float x1 = checkCoords.X;
            float y1 = checkCoords.Y;

            if (Mathf.Abs(y1 - y0) < Mathf.Abs(x1 - x0))
            {
                if (x1 < x0)
                {
                    reverse = true;
                    (x0, x1) = (x1, x0);
                    (y0, y1) = (y1, y0);
                }

                float dx = x1 - x0;
                float dy = y1 - y0;
                float m = (dx != 0) ? dy / dx : 1;

                for (int i = 0; i <= Mathf.Abs(Mathf.RoundToInt(dx)); i++)
                {
                    float x = x0 + i;
                    float y = y0 + i * m;
                    int ix = (int)x;
                    int iy = (int)y;
                    float dist = y - (float)iy;
                    if (dist < .5f)
                    {
                        Vector2I newCoord = new Vector2I(ix, iy);

                        if (tileLine.Contains(newCoord))
                        {
                            continue;
                        }

                        if (reverse)
                        {
                            tileLine.Insert(0, newCoord);
                        }
                        else
                        {
                            tileLine.Add(newCoord);
                        }
                    }
                    else
                    {
                        Vector2I newCoord = new Vector2I(ix, iy + 1);

                        if (tileLine.Contains(newCoord))
                        {
                            continue;
                        }

                        if (reverse)
                        {
                            tileLine.Insert(0, newCoord);
                        }
                        else
                        {
                            tileLine.Add(newCoord);
                        }
                    }
                }
            }
            else
            {
                if (y1 < y0)
                {
                    reverse = true;
                    (x0, x1) = (x1, x0);
                    (y0, y1) = (y1, y0);
                }

                float dx = x1 - x0;
                float dy = y1 - y0;
                float m = (dy != 0) ? dx / dy : 1;

                for (int i = 0; i <= Mathf.Abs(Mathf.RoundToInt(dy)); i++)
                {
                    float x = x0 + i * m;
                    float y = y0 + i;
                    int ix = (int)x;
                    int iy = (int)y;
                    float dist = x - (float)ix;
                    //add is within bounds
                    if (dist < .5f)
                    {
                        Vector2I newCoord = new Vector2I(ix, iy);

                        if (tileLine.Contains(newCoord))
                        {
                            continue;
                        }

                        if (reverse)
                        {
                            tileLine.Insert(0, newCoord);
                        }
                        else
                        {
                            tileLine.Add(newCoord);
                        }
                    }
                    else
                    {
                        Vector2I newCoord = new Vector2I(ix + 1, iy);

                        if (tileLine.Contains(newCoord))
                        {
                            continue;
                        }

                        if (reverse)
                        {
                            tileLine.Insert(0, newCoord);
                        }
                        else
                        {
                            tileLine.Add(newCoord);
                        }
                    }
                }
            }
            //check for visible tiles
            tileLine.RemoveAt(0);

            float totalVisionCost = 0f;

            foreach (Vector2I tile in tileLine)
            {
                if (!_grid.IsWithinBounds(tile))
                {
                    continue;
                }

                totalVisionCost += _map.GetTileVisionCost(tile);

                if (IsOccupied(tile) && !_unitManager.CanPass(unit.unitGroup, _units[tile].unitGroup))
                {
                    totalVisionCost += 2f;
                }

                GD.Print("uses base stat for vision");
                if ((float)unit.unitStats.GetBaseStat(UnitStatEnum.VISION) >= totalVisionCost)
                {
                    visibleTiles.Add(tile);
                }
                else
                {
                    blockingTiles.Add(tile);
                    break;
                }
            }
        }

        //visible tiles get revealed
        Vector2[] outputVision = new Vector2[visibleTiles.Count()];
        Vector2[] outputBlocked = new Vector2[blockingTiles.Count()];

        for (int i = 0; i < visibleTiles.Count(); i++)
        {
            outputVision[i] = visibleTiles[i];
        }

        if (blockingTiles.Count() > 0)
        {
            for (int i = 0; i < blockingTiles.Count(); i++)
            {
                outputBlocked[i] = blockingTiles[i];
            }
        }
        ShowVision(unit, outputVision, outputBlocked);
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

                if (_cellRevealedBy[coords].Count <= 0 )
                {
                    if (_cellBlockedBy.ContainsKey(coords) && _cellBlockedBy[coords].Count > 0)
                    {
                        ChangeTileVisibilityState(TileVisibilityState.BLOCKING, coords);
                    }
                    else
                    {
                        ChangeTileVisibilityState(TileVisibilityState.UNSEEN, coords);
                    }
                    _cellRevealedBy.Remove(coords);
                }
            }
            if (!keepUnitKey)
            {
                _unitVision.Remove(unit);
            }
        }

        if (_unitVisionBlocked.ContainsKey(unit))
        {
            foreach (Vector2 coords in _unitVisionBlocked[unit])
            {
                _cellBlockedBy[coords].Remove(unit);

                if (_cellBlockedBy[coords].Count <= 0)
                {
                    if (_cellRevealedBy.ContainsKey(coords) && _cellRevealedBy[coords].Count > 0)
                    {
                        ChangeTileVisibilityState(TileVisibilityState.VISIBLE, coords);
                    }
                    else
                    {
                        ChangeTileVisibilityState(TileVisibilityState.UNSEEN, coords);
                    }
                    _cellBlockedBy.Remove(coords);
                }
            }
            if (!keepUnitKey)
            {
                _unitVisionBlocked.Remove(unit);
            }
        }
    }

    /// <summary>
    /// reveals fog of war maps for units
    /// updates data structures
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="visibleCoords">coordinates that are visible</param>
    /// <param name="blockedCoords">coordinates that are blocked</param>
    public void ShowVision(Unit unit, Vector2[] visibleCoords, Vector2[] blockedCoords)
    {
        if (!_unitVision.ContainsKey(unit))
        {
            _unitVision.Add(unit, visibleCoords);
        }
        else
        {
            _unitVision[unit] = visibleCoords;
        }

        foreach (Vector2 coord in visibleCoords)
        {
            if (!_cellRevealedBy.ContainsKey(coord))
            {
                _cellRevealedBy.Add(coord, new List<Unit>());
            }
            _cellRevealedBy[coord].Add(unit);

            ChangeTileVisibilityState(TileVisibilityState.VISIBLE, coord);
        }

        if (!_unitVisionBlocked.ContainsKey(unit))
        {
            _unitVisionBlocked.Add(unit, blockedCoords);
        }
        else
        {
            _unitVisionBlocked[unit] = blockedCoords;
        }


        if (blockedCoords.Length > 0)
        {
            foreach (Vector2 coord in blockedCoords)
            {
                if (!_cellBlockedBy.ContainsKey(coord))
                {
                    _cellBlockedBy.Add(coord, new List<Unit>());
                }
                _cellBlockedBy[coord].Add(unit);

                if (!_cellRevealedBy.ContainsKey(coord))
                {
                    ChangeTileVisibilityState(TileVisibilityState.BLOCKING, coord);
                }
            }
        }
    }

    /// <summary>
    /// calls to chage the tiles visibility
    /// </summary>
    /// <param name="state">what state the tile is in</param>
    /// <param name="tileCoord">which tile</param>
    private void ChangeTileVisibilityState(TileVisibilityState state, Vector2 tileCoord)
    {
        switch (state)
        {
            case TileVisibilityState.UNSEEN:
                _fogOfWar.HideMapCell(tileCoord);
                _blockedOverlay.RemoveBlockCell(tileCoord);
                break;
            case TileVisibilityState.BLOCKING:
                _fogOfWar.RevealMapCell(tileCoord);
                _blockedOverlay.BlockCell(tileCoord);
                break;
            case TileVisibilityState.VISIBLE:
                _fogOfWar.RevealMapCell(tileCoord);
                _blockedOverlay.RemoveBlockCell(tileCoord);
                break;
        }
    }

    //---------- PROPERTIES ----------

    //note, make export if need to test outside of game board
    public GridResource grid
    {
        get { return _grid; }
    }

    public GridCursor gridCursor
    {
        get { return _gridCursor; }
    }

    public MenuStateMachine menuStateMachine
    {
        get { return _menuStateMachine; }
    }

    public Unit selectedUnit
    {
        get { return _selectedUnit; }
    }
}
