using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [05/20/2025]
 * [Unit Main Script]
 */

public partial class Unit : Node2D
{
    //maybe somewhere else, but will likely leave in this script
    private Vector2 _cell = Vector2.Zero;
    private bool _isSelected = false;

    [Export] private UnitPathMovement _unitPathMovement;
    private bool _unitCanWalk = false;
    private Vector2 _targetCell = Vector2.Zero;

    [Export] private UnitSprite _unitSprite;

    [Export] private UnitStats _unitStats;

    [Export] private UnitDirection _unitDirection;

    [Export] private UnitActionEconomy _unitActionEconomy;

    [Export] private NPCAiStateMachine _aiStateMachine;

    //animation player, but i might need to make a state machine
    //will keep in here for now to see how is selected works
    [Export] private AnimationPlayer _animPlayer;

    [Export] private GameBoard _gameBoard;
    public Action<GameBoard> CurrentGameBoard;

    //might take out since im going to lean twoards dnd action econome\y
    [Export] private bool _isWait = false;
    //temp, move to equiptment
    [Export] private int _attackRange = 1;

    [Export] private bool _isCommander = false;

    private UnitGroupEnum _unitGroup;

    /// <summary>
    /// sets unit's positon
    /// </summary>
    public override void _Ready()
    {
        if (_gameBoard != null)
        {
            CurrentGameBoard?.Invoke(_gameBoard);
        }
        else
        {
            GD.Print("WARNING: NO GAMEBOARD ON UNIT AT READY");
        }
        this.cell = _gameBoard.grid.CalculateGridCoordinates(Position);
        Position = _gameBoard.grid.CalculateMapPosition(cell);
    }

    /// <summary>
    /// calls functions that need to happen every frame
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        float fDelta = (float)delta;

        if (_unitPathMovement != null && _unitCanWalk)
        {
            _unitPathMovement.WalkUnit(fDelta, _targetCell);
        }
    }

    public void ChangeGameBoard(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;
        CurrentGameBoard?.Invoke(_gameBoard);
    }

    public bool IsAi()
    {
        return _aiStateMachine != null;
    }

    //--- PROPERTIES ---
    public Vector2 cell
    {
        set
        {
            if (_gameBoard.grid != null)
            {
                _cell = _gameBoard.grid.Clamp(value);
            }
        }
        get { return _cell; }
    }

    public Vector2 targetCell
    {
        set
        {
            if (_gameBoard.grid != null)
            {
                _targetCell = _gameBoard.grid.Clamp(value);
            }
        }
    }

    /// <summary>
    /// this also handles animation player of selected
    /// </summary>
    public bool isSelected
    {
        set
        {
            _isSelected = value;
            if (_animPlayer != null)
            {
                if (_isSelected)
                {
                    _animPlayer.Play("UnitTestSelected");
                }
                else
                {
                    _animPlayer.Play("UnitTestIdle");
                }
            }
            else
            {
                GD.Print("no AnimationPlayer Node for is selected to be set on");
            }
        }
        get { return _isSelected; }
    }

    public bool unitCanWalk
    {
        set
        {
            _unitCanWalk = value;
        }
    }

    public UnitStats unitStats
    {
        get { return _unitStats; }
    }

    public UnitPathMovement unitPathMovement
    {
        get { return _unitPathMovement; }
    }

    public UnitGroupEnum unitGroup
    {
        get { return _unitGroup; }
        set { _unitGroup = value; }
    }

    public UnitDirection unitDirection
    {
        get { return _unitDirection; }
    }

    public UnitActionEconomy unitActionEconomy
    {
        get { return _unitActionEconomy; }
    }

    public NPCAiStateMachine aiStateMachine
    {
        get { return _aiStateMachine; }
    }

    public bool isWait
    {
        get { return _isWait; }
    }

    public int attackRange
    {
        get { return _attackRange; }
    }

    public bool isCommander
    {
        get { return _isCommander; }
    }
}
