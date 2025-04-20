using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/20/2025]
 * [script for the cursor]
 */

public partial class GridCursor : Node2D
{
    //when pressed
    [Signal] public delegate void AcceptPressEventHandler(Vector2 cell);
    //when moved
    [Signal] public delegate void MovedEventHandler(Vector2 nextCell);

    [Export] private GridResource _grid;
    [Export] private Timer _timer;

    [Export] private float _uiCooldown = .1f;

    private Vector2 _cell = Vector2.Zero; //setget

    /// <summary>
    /// sets timer and first position
    /// </summary>
    public override void _Ready()
    {
        _timer.WaitTime = _uiCooldown;
        Position = _grid.CalculateMapPosition(_cell);
    }

    /// <summary>
    /// inputs for cursor
    /// 
    /// TODO: use input maps
    /// TODO: look into input device detection
    /// for mouse and keyboard or controller
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion input)
        {
            this.cell = _grid.CalculateGridCoordinates(input.Position);
        }
        //doesn't work, will be fix when looking into input map
        /*
        else if (@event.IsActionPressed("click"))
        {
            GD.Print("temp works");
            EmitSignal("AcceptPress", cell);
            GetViewport().SetInputAsHandled();
        }
        */

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
            Vector2 newCell = _grid.Clamp(value);
            if (newCell.IsEqualApprox(_cell))
            {
                return;
            }

            _cell = newCell;

            Position = _grid.CalculateMapPosition(_cell);
            EmitSignal("Moved");
            _timer.Start();
        }
        get { return _cell; }
    }
}
