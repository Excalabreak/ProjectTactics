using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [06/11/2025]
 * [script for the cursor]
 */

public partial class GridCursor : Node2D
{
    //when pressed
    [Signal] public delegate void AcceptPressEventHandler(Vector2 cell);
    //when moved
    [Signal] public delegate void MovedEventHandler(Vector2 nextCell);

    [Signal] public delegate void DeclineEventHandler();

    [Export] private GameBoard _gameBoard;

    [Export] private Timer _timer;
    [Export] private float _uiCooldown = .1f;

    private Vector2 _cell = Vector2.Zero; //setget

    //dont know if i will have an input manager, so it's here for now
    private bool _isMouse = false;
    private bool _dontEmitMoveSignal = false;

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
        else if (@event.IsActionPressed("ui_accept"))
        {
            EmitSignal("AcceptPress", cell);
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("Decline"))
        {
            EmitSignal("Decline");
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
            _isMouse = false;
            this.cell += Vector2.Up;
        }
        else if (@event.IsAction("ui_down"))
        {
            _isMouse = false;
            this.cell += Vector2.Down;
        }
        else if (@event.IsAction("ui_left"))
        {
            _isMouse = false;
            this.cell += Vector2.Left;
        }
        else if (@event.IsAction("ui_right"))
        {
            _isMouse = false;
            this.cell += Vector2.Right;
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
    /// warps the mouse w/o emiting a move signal
    /// </summary>
    /// <param name="screenPos">position of the screen for the mouse</param>
    public void WarpMouseWithoutSignal(Vector2 screenPos)
    {
        _dontEmitMoveSignal = true;
        Input.WarpMouse(screenPos);
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

            if (GetWindow().HasFocus() && !_isMouse)
            {
                Input.WarpMouse(this.GetGlobalTransformWithCanvas().Origin);
                _isMouse = false;
            }

            if (!_dontEmitMoveSignal)
            {
                EmitSignal("Moved", _cell);
            }
            else
            {
                _dontEmitMoveSignal = false;
            }
                _timer.Start();
        }
        get { return _cell; }
    }

    public bool isMouse
    {
        get { return _isMouse; }
    }
}
