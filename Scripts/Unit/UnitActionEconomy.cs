using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/24/2025]
 * [keeps track of actions unit can make]
 */

public partial class UnitActionEconomy : Node
{
    private GameBoard _gameBoard;

    [Export] private Unit _unit;
    [Export] private UnitStats _unitStats;
    private int _maxActions = 1;
    private int _actionsLeft = 1;

    public override void _Ready()
    {
        _unit.CurrentGameBoard += SetGameBoard;
    }

    /// <summary>
    /// checks if the unit has any action
    /// it can take including moving
    /// </summary>
    /// <returns>true if </returns>
    public bool HasAnyActions()
    {
        if (HasActions())
        {
            return true;
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
        _actionsLeft = _maxActions;
    }

    public bool HasActions()
    {
        return _actionsLeft > 0;
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
}
