using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/08/2025]
 * [keeps track of actions unit can make]
 */

public partial class UnitActionEconomy : Node
{
    private GameBoard _gameBoard;

    [Export] private Unit _unit;
    [Export] private UnitStats _unitStats;
    private int _maxActions = 1;
    private int _actionsLeft = 1;
    private float _currentMove = 6;

    public override void _Ready()
    {
        _unit.CurrentGameBoard += SetGameBoard;

        ResetActions();
    }

    /// <summary>
    /// checks if the unit has any action
    /// it can take including moving
    /// </summary>
    /// <returns>true if </returns>
    public bool CanTakeAnyActions()
    {
        if (HasActions())
        {
            if (CanAttackAction())
            {
                return true;
            }
        }

        if (_gameBoard.GetWalkableCells(_unit).Length > 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// called when an action is used
    /// </summary>
    public void UseAction()
    {
        _actionsLeft--;
    }

    /// <summary>
    /// resets the number of actions
    /// the unit has
    /// </summary>
    public void ResetActions()
    {
        _currentMove = (float)_unitStats.GetBaseStat(UnitStatEnum.MOVE);
        _actionsLeft = _maxActions;
    }

    /// <summary>
    /// subtracts the move cost when
    /// a unit is moving
    /// </summary>
    /// <param name="cost">amount to move to the next tile</param>
    public void UseMove(float moveCost)
    {
        _currentMove -= moveCost;
    }

    /// <summary>
    /// checks if there is an action
    /// the unit can take
    /// </summary>
    /// <returns>true if the unit has actions</returns>
    public bool HasActions()
    {
        return _actionsLeft > 0;
    }

    /// <summary>
    /// calls to check for units that the current unit can attack
    /// </summary>
    /// <returns></returns>
    public bool CanAttackAction()
    {
        return _gameBoard.CheckAreaForAttackableGroup(
            _unit.unitGroup, _gameBoard.FloodFill(_unit.cell, _unit.attackRange));
    }

    /// <summary>
    /// unsubs from event
    /// </summary>
    public override void _ExitTree()
    {
        _unit.CurrentGameBoard -= SetGameBoard;
    }

    /// <summary>
    /// sets _gameBoard
    /// </summary>
    /// <param name="gameBoard">v</param>
    private void SetGameBoard(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;
    }

    public float currentMove
    {
        get { return _currentMove; }
    }
}
