using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [08/02/2025]
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

    private bool _isPrecision = false;

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
        if (!_isMouse)
        {
            return;
        }
        Vector2 gridCoords = _gameBoard.grid.CalculateGridCoordinates(GetGlobalMousePosition());
        if (cell != gridCoords)
        {
            cell = gridCoords;
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
        else if (@event.IsActionPressed("Accept"))
        {
            EmitSignal("AcceptPress", cell);
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("Decline"))
        {
            EmitSignal("Decline");
            GetViewport().SetInputAsHandled();
        }

        //there definatly is a better way of doing this
        if (@event.IsActionPressed("Precision"))
        {
            _isPrecision = true;
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionReleased("Precision"))
        {
            _isPrecision = false;
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

        if (@event.IsAction("Up"))
        {
            _isMouse = false;
            this.cell += Vector2.Up;
        }
        else if (@event.IsAction("Down"))
        {
            _isMouse = false;
            this.cell += Vector2.Down;
        }
        else if (@event.IsAction("Left"))
        {
            _isMouse = false;
            this.cell += Vector2.Left;
        }
        else if (@event.IsAction("Right"))
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
        if (!_isMouse)
        {
            return;
        }
        Vector2 gridCoords = _gameBoard.grid.CalculateGridCoordinates(GetGlobalMousePosition());
        cell = gridCoords;
    }

    public void WarpMouseToUnitWithoutSignal(Unit unit)
    {
        if (!_isMouse)
        {
            return;
        }
        Vector2 mouseCoords = _gameBoard.grid.CalculateGridCoordinates(GetGlobalMousePosition());
        if (!mouseCoords.IsEqualApprox(unit.cell))
        {
            _dontEmitMoveSignal = true;
            Input.WarpMouse(unit.GetGlobalTransformWithCanvas().Origin);
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

            //was causing issues
            //check here if other wonky shit happens
            /*
            if (GetWindow().HasFocus() && !_isMouse)
            {
                Input.WarpMouse(this.GetGlobalTransformWithCanvas().Origin);
                _isMouse = false;
            }
            */

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

    public bool isPrecision
    {
        get { return _isPrecision; }
    }
}
