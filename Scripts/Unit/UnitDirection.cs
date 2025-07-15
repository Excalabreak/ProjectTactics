using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/09/2025]
 * [Manages any change in direction]
 */

public partial class UnitDirection : Node2D
{
    [Export] private Unit _unit;
    private GameBoard _gameBoard;

    private DirectionEnum _currentFacing = DirectionEnum.UP;
    public Action<DirectionEnum> UpdateCurrentFacing;

    public override void _Ready()
    {
        _unit.CurrentGameBoard += SetGameBoard;

        //this seems like it can be a problem later
        UpdateCurrentFacing?.Invoke(_currentFacing);
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

    public DirectionEnum currentFacing
    {
        get { return _currentFacing; }
        set
        {
            _currentFacing = value;
            _gameBoard.UpdateUnitVision(_unit);
            UpdateCurrentFacing?.Invoke(_currentFacing);
        }
    }
}
