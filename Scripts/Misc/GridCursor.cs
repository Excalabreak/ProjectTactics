using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [05/02/2025]
 * [script for the cursor]
 */

public partial class GridCursor : Node2D
{
    //when pressed
    [Signal] public delegate void AcceptPressEventHandler(Vector2 cell);
    //when released
    [Signal] public delegate void AcceptReleaseEventHandler(Vector2 cell);
    //when moved
    [Signal] public delegate void MovedEventHandler(Vector2 nextCell);

    [Export] private GameBoard _gameBoard;

    [Export] private Timer _timer;
    [Export] private float _uiCooldown = .1f;

    private Vector2 _cell = Vector2.Zero; //setget
    private bool _acceptHeld = false;

    //dont know if i will have an input manager, so it's here for now
    private bool _isMouse = false;

    /// <summary>
    /// sets timer and first position
    /// </summary>
    public override void _Ready()
    {
        _timer.WaitTime = _uiCooldown;
        cell = _gameBoard.grid.CalculateGridCoordinates(Position);
        Position = _gameBoard.grid.CalculateMapPosition(_cell);
    }

    public override void _Process(double delta)
    {
        if (_isMouse)
        {
            Vector2 gridCoords = _gameBoard.grid.CalculateGridCoordinates(GetGlobalMousePosition());
            if (cell != gridCoords)
            {
                cell = gridCoords;
            }
        }
    }

    /// <summary>
    /// inputs for cursor
    /// 
    /// for mouse and keyboard or controller
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        //inputs for accept
        if (@event is InputEventMouseMotion input)
        {
            _isMouse = true;
        }
        else if (@event.IsActionPressed("ui_accept", true))
        {
            if (!@event.IsEcho())
            {
                _acceptHeld = false;
                EmitSignal("AcceptPress", cell);
                GetViewport().SetInputAsHandled();
            }
            else
            {
                _acceptHeld = true;
            }
        }
        else if (@event.IsActionReleased("ui_accept"))
        {
            _acceptHeld = false;
            EmitSignal("AcceptRelease", cell);
            GetViewport().SetInputAsHandled();
        }

        //inputs for move
        bool shouldMove = @event.IsPressed();

        if (@event.IsEcho())
        {
            shouldMove = shouldMove && _timer.IsStopped();
        }

        if (!shouldMove)
        {
            return;
        }

        if (@event.IsAction("ui_up"))
        {
            this.cell += Vector2.Up;
            _isMouse = false;
        }
        else if (@event.IsAction("ui_down"))
        {
            this.cell += Vector2.Down;
            _isMouse = false;
        }
        else if (@event.IsAction("ui_left"))
        {
            this.cell += Vector2.Left;
            _isMouse = false;
        }
        else if (@event.IsAction("ui_right"))
        {
            this.cell += Vector2.Right;
            _isMouse = false;
        }
    }

    /// <summary>
    /// makes sure cursor is at right location on mouse
    /// </summary>
    public void ResetCursor()
    {
        if (_isMouse)
        {
            Vector2 gridCoords = _gameBoard.grid.CalculateGridCoordinates(GetGlobalMousePosition());
            cell = gridCoords;
        }
    }

    /// <summary>
    /// property for cell
    /// when setting:
    /// check if cell is viable to change
    /// if true, change pos and emit moved signal
    /// </summary>
    public Vector2 cell
    {
        set
        {
            Vector2 newCell = _gameBoard.grid.Clamp(value);
            if (newCell.IsEqualApprox(_cell))
            {
                return;
            }

            _cell = newCell;

            Position = _gameBoard.grid.CalculateMapPosition(_cell);

            EmitSignal("Moved", _cell);
            _timer.Start();
        }
        get { return _cell; }
    }

    public bool acceptHeld
    {
        get { return _acceptHeld; }
    }

    public bool isMouse
    {
        get { return _isMouse; }
    }
}
