using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
 * Last Updated: [08/26/2025]
 * [game board manages everything on the map]
 */

public partial class GameBoard : Node2D
{
    [ExportGroup("Components")]
    [Export] private GridResource _grid;
    [Export] private UnitWalkHighlight _unitWalkHighlights;
    [Export] public UnitPath _unitPath;
    [Export] private UnitManager _unitManager;
    [Export] private GridCursor _gridCursor;
    [Export] private Map _map;
    [Export] private FogOfWar _fogOfWar;
    [Export] private BlockedOverlay _blockedOverlay;
    [Export] private KnownUnitLocations _knownUnitLocationsTileMap;
    [Export] private TurnManager _turnManager;
    [Export] private UIManager _uiManager;
    [Export] private CombatManager _combatManager;

    [ExportGroup("Menu")]
    [Export] private MenuStateMachine _menuStateMachine;
    [Export] private PackedScene _actionMenu;
    [Export] private PackedScene _pauseMenu;
    [Export] private PackedScene _turnMenu;
    [Export] private PackedScene _tradeMenu;
    [Export] private PackedScene _itemMenu;
    [Signal] public delegate void SelectedMovedEventHandler();

    private ActionMenu _actionMenuInstance;
    private PauseScreen _pauseScreenInstance;
    private TurnMenu _turnMenuInstance;
    private TradeMenu _tradeMenuInstance;
    private ItemMenu _itemMenuInstance;

    [ExportGroup("MoveCost")]
    private Unit _selectedUnit;
    private Vector2[] _walkableCells;
    private Vector2[] _attackableCells;
    private float[,] _movementCosts;
    [Export] private float _unitPassCost = 2f;

    //all units, might want to split this up
    private System.Collections.Generic.Dictionary<Vector2, Unit> _units = new System.Collections.Generic.Dictionary<Vector2, Unit>();
    private List<Vector2> _knownUnitLocations = new List<Vector2>();

    //Dictionary for the fog of war
    private Dictionary<Unit, Vector2[]> _unitVision = new Dictionary<Unit, Vector2[]>();
    private Dictionary<Vector2, List<Unit>> _cellRevealedBy = new Dictionary<Vector2, List<Unit>>();
    private Dictionary<Unit, Vector2[]> _unitVisionBlocked = new Dictionary<Unit, Vector2[]>();
    private Dictionary<Vector2, List<Unit>> _cellBlockedBy = new Dictionary<Vector2, List<Unit>>();

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

        UnitEventManager.UnitDeathEvent += RemoveUnit;

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

            if (unit.unitGroup == UnitGroupEnum.PLAYER)
            {
                _knownUnitLocations.Add(unit.cell);
            }
        }

        //this makes sure all units are in _units before updating the unit vision
        foreach (KeyValuePair<Vector2, Unit> unit in _units)
        {
            UpdateUnitVision(unit.Value);
        }

        ResetKnownOccupied(_turnManager.currentTurn);

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

        UnitEventManager.UnitDeathEvent -= RemoveUnit;
    }

    //------------ INPUT ----------

    /// <summary>
    /// calls state machine to handle which cursor move
    /// method to use
    /// </summary>
    /// <param name="newCell">cell cursor is moving to</param>
    private void OnCursorMoved(Vector2 newCell)
    {
        //terrain info only here for prototype
        if (_grid.IsWithinBounds(newCell))
        {
            string terrainName = _map.GetTileTerrainName(newCell);
            float moveCost = _map.GetTileMoveCost(newCell);
            float visionCost = _map.GetTileVisionCost(newCell);
            _uiManager.ShowTerrainPanel(terrainName, moveCost, visionCost);
        }
        else
        {
            _uiManager.HideTerrainPanel();
        }

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
    /// 
    /// MODIFY FOR AI
    /// </summary>
    /// <param name="cell">cell to select unit</param>
    public void SelectUnit(Vector2 cell)
    {
        if (!_units.ContainsKey(cell))
        {
            return;
        }

        if (_turnManager.currentTurn != _units[cell].unitGroup)
        {
            return;
        }

        _selectedUnit = _units[cell];
        _selectedUnit.isSelected = true;

        _walkableCells = GetWalkableCells(_selectedUnit);
        _attackableCells = GetAttackableCells(_selectedUnit);

        if (_selectedUnit.unitGroup == UnitGroupEnum.PLAYER)
        {
            _unitWalkHighlights.DrawAttackHighlights(_attackableCells);
            _unitWalkHighlights.DrawWalkHighlights(_walkableCells);

            _unitPath.Initialize(_walkableCells, _selectedUnit.cell);
        }
    }

    /// <summary>
    /// calls state machine to display information
    /// </summary>
    /// <param name="cell">cell to display info</param>
    public void HoverDisplay(Vector2 cell)
    {
        _menuStateMachine.currentState.OnHover(cell);
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
        _uiManager.HideStatsPanel();
        _uiManager.HideUnitInventory();
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
    /// MODIFY FOR AI
    /// </summary>
    /// <param name="newCell"></param>
    public async void MoveSelectedUnit(Vector2 newCell)
    {
        if (!IsValidMoveLoc(newCell))
        {
            DeselectSelectedUnit();
            ClearSelectedUnit();
            EmitSignal("SelectedMoved");
            return;
        }

        DeselectSelectedUnit();
        OnlyPlayerTurnMenuStateTransition("MenuBlankState");
        _selectedUnit.unitPathMovement.SetWalkPath(_unitPath.currentPath);

        await ToSignal(_selectedUnit.unitPathMovement, "WalkFinished");

        ClearSelectedUnit();
        _walkableCells = new Vector2[0];
        EmitSignal("SelectedMoved");
    }

    /// <summary>
    /// removes unit for _units
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitLocation(Unit unit)
    {
        if (!_units.ContainsKey(unit.cell))
        {
            return;
        }
        if (_units[unit.cell] != unit)
        {
            return;
        }

        _units.Remove(unit.cell);
    }

    public void AddUnitLocation(Unit unit)
    {
        if (_units.ContainsKey(unit.cell))
        {
            return;
        }

        _units.Add(unit.cell, unit);
    }

    /// <summary>
    /// removes from knownUnitLocation
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveKnownUnitLocation(Unit unit)
    {
        if (!_knownUnitLocations.Contains(unit.cell))
        {
            return;
        }
        _knownUnitLocations.Remove(unit.cell);
    }

    /// <summary>
    /// adds a unit to known unit location
    /// </summary>
    /// <param name="unit"></param>
    public void AddKnownUnitLocation(Unit unit)
    {
        if (_knownUnitLocations.Contains(unit.cell))
        {
            return;
        }

        _knownUnitLocations.Add(unit.cell);
    }

    /// <summary>
    /// checks if cell coords is
    /// a valid end point
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool IsValidMoveLoc(Vector2 cell)
    {
        if (_selectedUnit == null)
        {
            return false;
        }
        if (_walkableCells == null || _walkableCells.Length <= 0)
        {
            return false;
        }
        if (!_walkableCells.Contains(cell))
        {
            return false;
        }
        if (IsKnownOccupied(cell) && (cell != _selectedUnit.cell || _unitPath.currentPath.Length <= 1))
        {
            return false;
        }

        return true;
    }

    //---------- MENU CURSOR ACCEPT -----------

    /// <summary>
    /// selects unit and brings up menu if needed
    /// </summary>
    /// <param name="cell"></param>
    public void MenuUnSelectedStateAccept(Vector2 cell)
    {
        //need to add a condition for enemy units
        if (_selectedUnit == null && _units.ContainsKey(cell) && _units[cell].unitGroup == UnitGroupEnum.PLAYER)
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
        if (!IsValidMoveLoc(cell))
        {
            DeselectSelectedUnit();
            ClearSelectedUnit();
            _walkableCells = new Vector2[0];
            OnlyPlayerTurnMenuStateTransition("MenuUnSelectedState");
            return;
        }

        MoveSelectedUnit(cell);
        _unitWalkHighlights.Clear();
        await ToSignal(this, "SelectedMoved");

        OnlyPlayerTurnMenuStateTransition("MenuUnSelectedState");
        MenuUnSelectedStateAccept(cell);
    }

    /// <summary>
    /// selects unit for attack
    /// </summary>
    /// <param name="cell"></param>
    public void MenuAttackStateAccept(Vector2 cell)
    {
        //maybe change to IsKnownOccupied(),
        //but might want to keep blind swings
        if (!IsOccupied(cell))
        {
            return;
        }

        Unit opposingUnit = _units[cell];
        if (!_unitManager.CanAttack(_selectedUnit.unitGroup, opposingUnit.unitGroup))
        {
            return;
        }

        Vector2[] attackableCells = RangeFloodFill(_selectedUnit.cell, _selectedUnit.unitInventory.equiptWeapon.minRange, _selectedUnit.unitInventory.equiptWeapon.maxRange);
        if (!attackableCells.Contains(cell))
        {
            return;
        }

        //very basic combat for now
        _unitWalkHighlights.Clear();
        _uiManager.HideBattlePredictions();

        _combatManager.UnitCombat(_selectedUnit, opposingUnit);

        DeselectSelectedUnit();
        ClearSelectedUnit();
        OnlyPlayerTurnMenuStateTransition("MenuUnSelectedState");
    }

    /// <summary>
    /// selects unit for trade
    /// </summary>
    /// <param name="cell">cell to trade with</param>
    public void MenuTradeStateAccept(Vector2 cell)
    {
        if (_selectedUnit == null)
        {
            return;
        }

        if (!IsOccupied(cell))
        {
            return;
        }

        Unit otherUnit = _units[cell];

        if (_selectedUnit.unitGroup != otherUnit.unitGroup)
        {
            return;
        }

        int tradeRange = 1;
        if (!FloodFill(_selectedUnit.cell, tradeRange).Contains(cell))
        {
            return;
        }
        
        _tradeMenuInstance = _tradeMenu.Instantiate() as TradeMenu;
        _tradeMenuInstance.SetUpTradeMenu(_selectedUnit, otherUnit);
        AddChild(_tradeMenuInstance);
    }

    //---------- MENU CURSOR MOVE ----------

    /// <summary>
    /// shows the hover display for unit on new cell
    /// </summary>
    /// <param name="newCell">new cell</param>
    public void MenuUnSelectedStateCursorMove(Vector2 newCell)
    {
        _unitWalkHighlights.Clear();
        if (IsKnownOccupied(newCell))
        {
            HoverDisplay(newCell);
        }
        else
        {
            _uiManager.HideStatsPanel();
            _uiManager.HideUnitInventory();
        }
    }

    /// <summary>
    /// shows hover display when checking combat
    /// </summary>
    /// <param name="newCell"></param>
    public void MenuAttackStateCursorMove(Vector2 newCell)
    {
        if (_selectedUnit == null)
        {
            _uiManager.HideBattlePredictions();
            return;
        }
        if (_selectedUnit.cell == newCell)
        {
            _uiManager.HideBattlePredictions();
            return;
        }
        if (!_units.ContainsKey(newCell))
        {
            _uiManager.HideBattlePredictions();
            return;
        }
        if (!IsKnownOccupied(newCell))
        {
            _uiManager.HideBattlePredictions();
            return;
        }
        if (RangeFloodFill(_selectedUnit.cell, _selectedUnit.unitInventory.equiptWeapon.minRange, _selectedUnit.unitInventory.equiptWeapon.maxRange).Contains(newCell))
        {
            HoverDisplay(newCell);
        }
        else
        {
            _uiManager.HideBattlePredictions();
        }
    }

    /// <summary>
    /// draws path for move action
    /// </summary>
    /// <param name="newCell"></param>
    public void MenuMoveStateCursorMove(Vector2 newCell)
    {
        if (!_gridCursor.isPrecision)
        {
            _unitPath.DrawAutoPath(_selectedUnit.cell, newCell);
            return;
        }

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

        Vector2I intNewCell = new Vector2I(Mathf.RoundToInt(newCell.X), Mathf.RoundToInt(newCell.Y));
        List<Vector2I> path = new List<Vector2I>();
        if (_unitPath.GetIntCurrentPath().Length > 0)
        {
            path.AddRange(_unitPath.GetIntCurrentPath());
            path.RemoveAt(0);
        }
        if (_map.GetPathMoveCost(path.ToArray()) + _map.GetTileMoveCost(intNewCell)
            > _selectedUnit.unitActionEconomy.currentMove)
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
    /// 
    /// NOTE: might want to take this out of game board
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
        else if (IsInstanceValid(_turnMenuInstance))
        {
            _turnMenuInstance.OnCancelPressed();
        }
        else if (IsInstanceValid(_tradeMenuInstance))
        {

        }
        ResetMenu();
    }

    /// <summary>
    /// allows players to start over when drawing the unit's
    /// walk path
    /// </summary>
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

    //---------- HOVER DISPLAY ----------

    /// <summary>
    /// base hover display that displays unit:
    /// walk range and max attack ranges
    /// </summary>
    /// <param name="cell"></param>
    public void BaseHoverDisplay(Vector2 cell)
    {
        if (!IsKnownOccupied(cell))
        {
            return;
        }
        if (!_units.ContainsKey(cell))
        {
            return;
        }
        Unit curUnit = _units[cell];

        _uiManager.ShowUnitStats(curUnit);
        _uiManager.ShowUnitInventory(curUnit);

        _walkableCells = GetWalkableCells(curUnit);
        _attackableCells = GetAttackableCells(curUnit);

        _unitWalkHighlights.DrawAttackHighlights(_attackableCells);
        _unitWalkHighlights.DrawWalkHighlights(_walkableCells);
    }

    /// <summary>
    /// shows the combat predictions if fights unit in cell
    /// 
    /// NOTE: will likely need to add combat confirmation
    /// when adding equiptment
    /// </summary>
    /// <param name="cell">cell</param>
    public void CombatHoverDisplay(Vector2 cell)
    {
        Unit initUnit = _selectedUnit;
        Unit targetUnit = _units[cell];

        UnitStats playerStats = initUnit.unitStats;
        UnitStats enemyStats = targetUnit.unitStats;

        int playerDamage = combatManager.CalculateDamage(initUnit, targetUnit);
        int enemyDamage = 0;

        if (_combatManager.CanReach(targetUnit, initUnit))
        {
            enemyDamage = combatManager.CalculateDamage(targetUnit, initUnit);
        }

        int playerAcc = _combatManager.CalculateHitRate(initUnit, targetUnit);
        int enemyAcc = _combatManager.CalculateHitRate(targetUnit, initUnit);

        int playerCrit = _combatManager.CalculateCritRate(initUnit, targetUnit);
        int enemyCrit = _combatManager.CalculateCritRate(targetUnit, initUnit);

        //basic combat
        //logic for magic numbers don't exist yet
        //100 for acc, 0 for crits
        _uiManager.ShowBattlePredictions(playerStats.currentHP, enemyStats.currentHP,
            playerDamage, enemyDamage, playerAcc, enemyAcc, playerCrit, enemyCrit);
    }

    //---------- OTHER MENU ----------

    public void SpawnItemMenu()
    {
        _itemMenuInstance = _itemMenu.Instantiate() as ItemMenu;
        if (_selectedUnit != null)
        {
            _itemMenuInstance.UpdateButtonText(_selectedUnit.unitInventory);
        }
        AddChild(_itemMenuInstance);
    }

    /// <summary>
    /// resets menu
    /// </summary>
    public void ResetMenu()
    {
        DeselectSelectedUnit();
        ClearSelectedUnit();
        OnlyPlayerTurnMenuStateTransition("MenuUnSelectedState");
    }

    /// <summary>
    /// shows the turn menu
    /// 
    /// here for future if i want the buttons
    /// closer to the unit
    /// </summary>
    public void SpawnTurnMenu()
    {
        _turnMenuInstance = _turnMenu.Instantiate() as TurnMenu;
        AddChild(_turnMenuInstance);
    }

    /// <summary>
    /// calls to hide battleUI for state machine
    /// </summary>
    public void HideBattleUI()
    {
        _uiManager.HideBattlePredictions();
    }

    /// <summary>
    /// calls to hide stats ui for state machine
    /// </summary>
    public void HideStatsUI()
    {
        _uiManager.HideStatsPanel();
    }

    /// <summary>
    /// moves the grid cursor back onto the selected unit
    /// </summary>
    public void ResetMovePath()
    {
        if (_gridCursor.isMouse)
        {
            _gridCursor.WarpMouseToUnitWithoutSignal(_selectedUnit);
        }
        else
        {
            _gridCursor.cell = _selectedUnit.cell;
        }
    }

    //---------- END TURN ----------

    /// <summary>
    /// changes the turn
    /// </summary>
    public void EndTurn()
    {
        _turnManager.NextTurn();

        ResetKnownOccupied(_turnManager.currentTurn);

        foreach (Unit unit in _unitManager.GetGroupUnits(_turnManager.currentTurn))
        {
            unit.unitActionEconomy.ResetActions();
        }

        if (_turnManager.currentTurn == UnitGroupEnum.PLAYER)
        {
            _menuStateMachine.TransitionTo("MenuUnSelectedState");
        }
        else
        {
            _menuStateMachine.TransitionTo("MenuBlankState");
            AiTurn(_turnManager.currentTurn);
        }
    }

    /// <summary>
    /// clears unit and adds the units of the turn
    /// and adds units in vision
    /// </summary>
    /// <param name="turnGroup">group</param>
    private void ResetKnownOccupied(UnitGroupEnum turnGroup)
    {
        _knownUnitLocations = new List<Vector2>();
        Unit[] currentGroup = _unitManager.GetGroupUnits(turnGroup);
        _knownUnitLocationsTileMap.Clear();

        foreach (Unit unit in currentGroup)
        {
            _knownUnitLocations.Add(unit.cell);
        }

        //second for each loop to make sure all units are in known units
        foreach (Unit unit in currentGroup)
        {
            if (_unitVision.ContainsKey(unit))
            {
                foreach (Vector2 cell in _unitVision[unit])
                {
                    if (_knownUnitLocations.Contains(cell))
                    {
                        continue;
                    }
                    if (IsOccupied(cell))
                    {
                        _knownUnitLocations.Add(cell);
                    }
                }
            }
            if (_unitVisionBlocked.ContainsKey(unit))
            {
                foreach (Vector2 cell in _unitVisionBlocked[unit])
                {
                    if (_knownUnitLocations.Contains(cell))
                    {
                        continue;
                    }
                    if (IsOccupied(cell))
                    {
                        _knownUnitLocations.Add(cell);
                    }
                }
            }
        }
    }

    /// <summary>
    /// removes unit from game
    /// </summary>
    /// <param name="unit">unit</param>
    private void RemoveUnit(Unit unit)
    {
        Vector2 cell = unit.cell;
        if (_units.ContainsKey(cell))
        {
            _units.Remove(cell);
        }
        if (_knownUnitLocations.Contains(cell))
        {
            _knownUnitLocations.Remove(cell);
        }
        if (_unitVision.ContainsKey(unit) || _unitVisionBlocked.ContainsKey(unit))
        {
            HideVision(unit);
        }
        MovingUnitVisionUpdate(unit, cell);
    }

    //---------- BASIC AI ----------

    /// <summary>
    /// goes through all ai and have them perform
    /// their logic
    /// NOTE: Very simple now, will grow later
    /// </summary>
    /// <param name="group">which ai group is running</param>
    private async void AiTurn(UnitGroupEnum group)
    {
        Unit[] units = _unitManager.GetGroupUnits(group);
        //loops through each unit and loops their logic until 
        //they exaust all their actions

        foreach (Unit unit in units)
        {
            if (IsInstanceValid(unit) && unit.IsAi())
            {
                unit.aiStateMachine.DoLogic();
                if (unit.aiStateMachine.needsAwait)
                {
                    await ToSignal(unit.aiStateMachine, "UnitFinished");
                    unit.aiStateMachine.ResetNeedsAwait();
                }
            }
        }
        EndTurn();
    }

    /// <summary>
    /// gets the closest unit from cell
    /// that is of a specific group
    /// </summary>
    /// <param name="group">group to look for</param>
    /// <param name="cell">cell to search from</param>
    /// <returns></returns>
    public Vector2 ClosestUnitPosition(UnitGroupEnum group, Vector2 cell)
    {
        Unit[] searchUnits = _unitManager.GetGroupUnits(group);

        if (searchUnits.Length <= 0)
        {
            GD.Print("no units for ClosestUnitPosition");
            return Vector2.Zero;
        }

        Vector2 currentClosest = new Vector2(-1, -1);

        List<Vector2> path = new List<Vector2>();
        float currentMoveCost = 999999;
        for (int i = 0; i < searchUnits.Length; i++)
        {
            path.Clear();

            path.AddRange(DijkstraPathFinding(cell, searchUnits[i].cell));
            path.RemoveAt(0);
            path.RemoveAt(path.Count - 1);
            if (path.Count <= 0)
            {
                currentClosest = searchUnits[i].cell;
                break;
            }

            float moveCost = _map.GetPathMoveCost(path.ToArray());
            if (moveCost < currentMoveCost)
            {
                currentClosest = searchUnits[i].cell;
                currentMoveCost = moveCost;
            }
        }

        if (currentClosest == new Vector2(-1, -1))
        {
            GD.Print("No close units found in ClosestUnitPosition");
        }
        return currentClosest;
    }

    /// <summary>
    /// draws autopath for ai unit
    /// </summary>
    /// <param name="start">starting cell</param>
    /// <param name="end">ending cell</param>
    public void DrawAutoPathForAi(Vector2 start, Vector2 end)
    {
        _unitPath.DrawAutoPath(start, end);
    }

    //---------- DISPLAY HIGHLIGHTS ----------

    /// <summary>
    /// shows the attack range of unit
    /// </summary>
    public void ShowCurrentAttackRange(Unit unit)
    {
        _unitWalkHighlights.Clear();
        _unitWalkHighlights.DrawAttackHighlights(RangeFloodFill(unit.cell, unit.unitInventory.equiptWeapon.minRange, unit.unitInventory.equiptWeapon.maxRange));
    }

    /// <summary>
    /// shows cells units can trade with
    /// </summary>
    /// <param name="cell">unit</param>
    public void ShowTradeCells(Vector2 cell)
    {
        _unitWalkHighlights.Clear();

        int tradeRange = 1;
        _unitWalkHighlights.DrawAttackHighlights(FloodFill(cell, tradeRange));
    }

    /// <summary>
    /// Returns an array of cells a given unit can walk using the dijkasta algorithm
    /// </summary>
    /// <param name="unit">selected unit</param>
    /// <returns>array of coords that the unit can walk</returns>
    public Vector2[] GetWalkableCells(Unit unit)
    {
        float maxDistance = unit.unitActionEconomy.currentMove;

        if (_turnManager.currentTurn != unit.unitGroup)
        {
            maxDistance = unit.unitActionEconomy.maxMove;
        }

        return DijkstaFill(unit.cell, maxDistance, false);
    }

    /// <summary>
    /// gets the cells a unit can attack
    /// NOTE: inefficient, optimize later
    /// </summary>
    /// <param name="unit">selected unit</param>
    /// <returns>array of cell coordinates</returns>
    public Vector2[] GetAttackableCells(Unit unit)
    {
        List<Vector2> attackableCells = new List<Vector2>();

        float maxDistance = unit.unitActionEconomy.currentMove;

        if (_turnManager.currentTurn != unit.unitGroup)
        {
            maxDistance = unit.unitActionEconomy.maxMove;
        }

        Vector2[] realWalkableCells = DijkstaFill(unit.cell, maxDistance, true);

        foreach (Vector2 curCell in realWalkableCells)
        {
            attackableCells.AddRange(RangeFloodFill(curCell, unit.unitInventory.equiptWeapon.minRange, unit.unitInventory.equiptWeapon.maxRange));
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
    public Vector2[] FloodFill(Vector2 cell, int maxDistance)
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
    /// flood fills based on cell coordinates and max distance
    /// then does a second flood fill to remove cells based on min cells
    /// </summary>
    /// <param name="cell">starting point</param>
    /// <param name="minDistance">inclusive min distance</param>
    /// <param name="maxDistance">inclusive max distance</param>
    /// <returns>array of coords</returns>
    public Vector2[] RangeFloodFill(Vector2 cell, int minDistance, int maxDistance)
    {
        if (minDistance <= 1)
        {
            return FloodFill(cell, maxDistance);
        }

        return FloodFill(cell, maxDistance).Except(FloodFill(cell, (minDistance-1))).ToArray();
    }

    /// <summary>
    /// gets walkable units with move cost
    /// NOTE: tutorial said this is quick and dirty
    /// </summary>
    /// <param name="cell">unit coords</param>
    /// <param name="maxDistance">unit move stat</param>
    /// <param name="attackableCheck">check unit attack check</param>
    /// <returns>array of walkable units</returns>
    private Vector2[] DijkstaFill(Vector2 cell, float maxDistance, bool attackableCheck)
    {
        int y = Mathf.RoundToInt(grid.gridSize.Y);
        int x = Mathf.RoundToInt(grid.gridSize.X);

        Unit curUnit = _units[cell];
        List<Vector2> moveableCells = new List<Vector2>();
        bool[,] visited = new bool[y, x];
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
            visited[Mathf.RoundToInt(currentValue.Y), Mathf.RoundToInt(currentValue.X)] = true;

            foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
            {
                Vector2 coords = currentValue + DirectionManager.Instance.GetVectorDirection(dir);
                int coordsY = Mathf.RoundToInt(coords.Y);
                int coordsX = Mathf.RoundToInt(coords.X);
                if (!_grid.IsWithinBounds(coords))
                {
                    continue;
                }
                if (visited[coordsY, coordsX])
                {
                    continue;
                }

                tileCost = _movementCosts[coordsY, coordsX];
                distanceToNode = currentPriority + tileCost;

                //checks if units can pass eachother
                if (IsKnownOccupied(coords))
                {
                    if (!_unitManager.CanPass(curUnit.unitGroup, _units[coords].unitGroup))
                    {
                        distanceToNode = currentPriority + MAX_VALUE;
                    }
                    else if (!_units[coords].unitActionEconomy.moveAction && attackableCheck)
                    {
                        occupiedCells.Add(coords);
                    }
                }

                visited[coordsY, coordsX] = true;
                distances[coordsY, coordsX] = distanceToNode;

                if (distanceToNode <= maxDistance)
                {
                    previous[coordsY, coordsX] = currentValue;
                    moveableCells.Add(coords);
                    dijkstaQueue.Enqueue(coords, distanceToNode);
                }

            }
        }

        return moveableCells.Except(occupiedCells).ToArray();
    }

    /// <summary>
    /// uses dijkstra to pathfind between 2 points
    /// </summary>
    /// <param name="startCell">first coord</param>
    /// <param name="endCell">last coord</param>
    /// <param name="maxDistance">limit to distance</param>
    /// <returns>array of coords</returns>
    public Vector2[] DijkstraPathFinding(Vector2 startCell, Vector2 endCell, float maxDistance = -1)
    {
        int y = Mathf.RoundToInt(grid.gridSize.Y);
        int x = Mathf.RoundToInt(grid.gridSize.X);

        bool hasStartUnit = _units.TryGetValue(startCell, out Unit curUnit);

        List<Vector2> path = new List<Vector2>();

        bool[,] visited = new bool[y, x];
        float[,] distances = new float[y, x];
        Vector2[,] previous = new Vector2[y, x];

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                visited[i, j] = false;
                distances[i, j] = MAX_VALUE;
                previous[i, j] = new Vector2(-1,-1);
            }
        }

        DijkstraPriorityList dijkstaQueue = new DijkstraPriorityList();
        dijkstaQueue.Enqueue(startCell, 0);
        distances[Mathf.RoundToInt(startCell.Y), Mathf.RoundToInt(startCell.X)] = 0;

        float tileCost;
        float distanceToNode;

        while (!visited[Mathf.RoundToInt(endCell.Y), Mathf.RoundToInt(endCell.X)] && dijkstaQueue.Count() > 0)
        {
            dijkstaQueue.TryDequeue(out Vector2 currentValue, out float currentPriority);
            visited[Mathf.RoundToInt(currentValue.Y), Mathf.RoundToInt(currentValue.X)] = true;


            foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
            {
                Vector2 coords = currentValue + DirectionManager.Instance.GetVectorDirection(dir);
                int coordsY = Mathf.RoundToInt(coords.Y);
                int coordsX = Mathf.RoundToInt(coords.X);

                if (!_grid.IsWithinBounds(coords))
                {
                    continue;
                }
                if (visited[coordsY, coordsX])
                {
                    continue;
                }

                tileCost = _movementCosts[coordsY, coordsX];
                distanceToNode = currentPriority + tileCost;

                //checks if units can pass eachother
                if (IsKnownOccupied(coords))
                {
                    if (hasStartUnit && !_unitManager.CanPass(curUnit.unitGroup, _units[coords].unitGroup))
                    {
                        distanceToNode = currentPriority + MAX_VALUE;
                    }
                }
                if (distanceToNode < distances[coordsY, coordsX]
                    || previous[coordsY, coordsX] == new Vector2(-1, -1))
                {
                    distances[coordsY, coordsX] = distanceToNode;
                    previous[coordsY, coordsX] = currentValue;

                    if (dijkstaQueue.ContainsValue(coords))
                    {
                        dijkstaQueue.ChangePriority(coords, distanceToNode);
                    }
                    else
                    {
                        dijkstaQueue.Enqueue(coords, distanceToNode);
                    }
                }

            }
        }
        path.Add(endCell);
        Vector2 current = endCell;
        while (current != startCell)
        {
            current = previous[Mathf.RoundToInt(current.Y), Mathf.RoundToInt(current.X)];
            path.Add(current);
        }
        path.Reverse();


        if (maxDistance > -1)
        {
            List<Vector2> check = new List<Vector2>();
            check.AddRange(path);
            check.RemoveAt(0);

            if (_map.GetPathMoveCost(check.ToArray()) > maxDistance)
            {
                path = new List<Vector2>();
            }
        }

        return path.ToArray();
    }

    /// <summary>
    /// Returns `true` if the cell is occupied by a unit.
    /// </summary>
    /// <param name="cell">coordinates of grid that are occupied</param>
    /// <returns>true if cell is occupied</returns>
    public bool IsOccupied(Vector2 cell)
    {
        return _units.ContainsKey(cell);
    }

    /// <summary>
    /// like IsOccupied, but will return false if the player doesn't know
    /// a unit is in the fog of war
    /// </summary>
    /// <param name="cell">coordinates on grid</param>
    /// <returns>true if the player should know 100% that a unit is occupying the cell</returns>
    public bool IsKnownOccupied(Vector2 cell)
    {
        return _knownUnitLocations.Contains(cell);
    }

    /// <summary>
    /// like IsOccupied, but will return true if the player doesn't know
    /// a unit is in the fog of war
    /// </summary>
    /// <param name="cell">coordinates on grid</param>
    /// <returns>true if the player should know 100% that a unit is occupying the cell</returns>
    private bool IsKnownOccupied(Vector2I cell)
    {
        return _knownUnitLocations.Contains(new Vector2(cell.X, cell.Y));
    }

    /// <summary>
    /// checks if unit can pass through a tile
    /// </summary>
    /// <param name="unit">unit checking</param>
    /// <param name="tile">tile passing</param>
    /// <returns>true if the unit can pass tile</returns>
    public bool CheckCanPass(Unit unit, Vector2 tile)
    {
        if (!IsOccupied(tile))
        {
            return true;
        }

        if (_unitManager.CanPass(unit.unitGroup, _units[tile].unitGroup))
        {
            return true;
        }

        return false;
    }

    //---------- DETECTION ----------

    /// <summary>
    /// checks if a unit of group
    /// can attack something
    /// </summary>
    /// <param name="group">group of attacking unit</param>
    /// <param name="area">array of coords</param>
    /// <returns>true if group can attack something</returns>
    public bool CheckAreaForAttackableGroup(UnitGroupEnum group, Vector2[] area)
    {
        foreach (Vector2 coord in area)
        {
            if (_units.ContainsKey(coord) && _unitManager.CanAttack(group, _units[coord].unitGroup))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// gets a unit of a specific from area
    /// </summary>
    /// <param name="attackingGroup">group of attacking unit</param>
    /// <param name="area">coords to search</param>
    /// <returns></returns>
    public Unit GetAttackableUnitFromArea(UnitGroupEnum attackingGroup, Vector2[] area)
    {
        foreach (Vector2 coord in area)
        {
            if (_units.ContainsKey(coord) && _unitManager.CanAttack(attackingGroup, _units[coord].unitGroup))
            {
                return _units[coord];
            }
        }

        return null;
    }

    /// <summary>
    /// checks if a unit group is in
    /// an area
    /// </summary>
    /// <param name="group">group to check for</param>
    /// <param name="area">array of coords</param>
    /// <returns>true if unit is in there</returns>
    public bool CheckAreaForGroup(UnitGroupEnum group, Vector2[] area)
    {
        foreach (Vector2 coord in area)
        {
            if (_units.ContainsKey(coord) && _units[coord].unitGroup == group)
            {
                return true;
            }
        }

        return false;
    }

    //---------- VISION/FOG OF WAR ----------

    /// <summary>
    /// updates the PLAYER unit vision on game map
    /// AFTER THE UNIT HAS MOVED
    /// NOTE: separate out getting tiles and showing tiles for ai
    /// </summary>
    /// <param name="unit">unit being updated</param>
    public void UpdateUnitVision(Unit unit)
    {
        //returns on non player units since only the player can see their units
        if (unit.unitGroup == UnitGroupEnum.PLAYER)
        {
            //hides unit's old vision
            //adds unit to vision dictionary if not there
            HideVision(unit, true);
        }

        //Vector2I for which tiles get checked
        Vector2I startingCell = new Vector2I(Mathf.RoundToInt(unit.cell.X), Mathf.RoundToInt(unit.cell.Y));

        //list of tiles
        List<Vector2I> checkTiles = new List<Vector2I>();
        List<Vector2I> visibleTiles = new List<Vector2I>();
        List<Vector2I> blockingTiles = new List<Vector2I>();

        visibleTiles.Add(startingCell);

        int r = unit.unitStats.GetStat(UnitStatEnum.VISION);
        
        //gets the furthest the unit can see
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

                if (!CheckCanPass(unit, tile))
                {
                    totalVisionCost += _unitPassCost;

                    //checks if unit location is known
                    //move this to a seperate
                    if (!IsKnownOccupied(tile))
                    {
                        _knownUnitLocations.Add(new Vector2(tile.X, tile.Y));
                    }
                }

                if ((float)unit.unitStats.GetStat(UnitStatEnum.VISION) >= totalVisionCost)
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

        //might need to add npc units to show and hide vision
        if (unit.unitGroup == UnitGroupEnum.PLAYER)
        {
            //might be where we set up different cases for groups
            //visible tiles get revealed
            Vector2[] outputVision = new Vector2[visibleTiles.Count()];
            Vector2[] outputBlocked = new Vector2[blockingTiles.Count()];

            //NOTE: this is so vector2i are vector2
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

        
    }

    /// <summary>
    /// Hides vision of unit
    /// </summary>
    /// <param name="unit">unit being updated</param>
    public void HideVision(Unit unit, bool keepUnitKey = false)
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
    /// updates any unit vision from an enemy unit
    /// moving into fog of war
    /// </summary>
    /// <param name="unit">unit</param>
    /// <param name="cell">cell unit is on</param>
    public void MovingUnitVisionUpdate(Unit unit, Vector2 cell)
    {
        if (!_cellBlockedBy.ContainsKey(cell) && !_cellRevealedBy.ContainsKey(cell))
        {
            return;
        }
        List<Unit> check = new List<Unit>();
        List<Unit> alreadyChecked = new List<Unit>();
        alreadyChecked.Add(unit);

        if (_cellRevealedBy.ContainsKey(cell))
        {
            bool skip = false;
            if (_cellRevealedBy[cell].Count <= 1 && _cellRevealedBy[cell][0] == unit)
            {
                skip = true;
            }

            if (!skip)
            {
                check = _cellRevealedBy[cell];
                for (int i = 0; i < check.Count(); i++)
                {
                    if (alreadyChecked.Contains(check[i]))
                    {
                        continue;
                    }

                    alreadyChecked.Add(check[i]);
                    UpdateUnitVision(check[i]);
                }
            }
        }

        if (_cellBlockedBy.ContainsKey(cell))
        {
            if (_cellBlockedBy[cell].Count <= 1 && _cellBlockedBy[cell][0] == unit)
            {
                return;
            }

            check = _cellBlockedBy[cell];
            for (int i = 0; i < check.Count(); i++)
            {
                if (alreadyChecked.Contains(_cellBlockedBy[cell][i]))
                {
                    continue;
                }

                alreadyChecked.Add(check[i]);
                UpdateUnitVision(check[i]);
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
                if (IsKnownOccupied(tileCoord))
                {
                    _knownUnitLocationsTileMap.MarkKnownUnit(tileCoord);
                }
                break;
            case TileVisibilityState.BLOCKING:
                _fogOfWar.RevealMapCell(tileCoord);
                _blockedOverlay.BlockCell(tileCoord);
                _knownUnitLocationsTileMap.RemoveMarkedUnit(tileCoord);
                break;
            case TileVisibilityState.VISIBLE:
                _fogOfWar.RevealMapCell(tileCoord);
                _blockedOverlay.RemoveBlockCell(tileCoord);
                _knownUnitLocationsTileMap.RemoveMarkedUnit(tileCoord);
                break;
        }
    }
    //---------- MISC ----------

    /// <summary>
    /// transition menu state machine only on player turn
    /// </summary>
    /// <param name="state">state to transition to</param>
    private void OnlyPlayerTurnMenuStateTransition(string state)
    {
        if (_turnManager.currentTurn == UnitGroupEnum.PLAYER)
        {
            menuStateMachine.TransitionTo(state);
        }
    }

    /// <summary>
    /// gets Unit from cell
    /// </summary>
    /// <param name="cell">cell to get unit</param>
    /// <returns>unit</returns>
    public Unit GetUnitFromCell(Vector2 cell)
    {
        if (!_units.ContainsKey(cell))
        {
            return null;
        }

        return _units[cell];
    }

    //---------- PROPERTIES ----------

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

    public Map map
    {
        get { return _map; }
    }

    public UnitManager unitManager
    {
        get { return _unitManager; }
    }

    public CombatManager combatManager
    {
        get { return _combatManager; }
    }
}
