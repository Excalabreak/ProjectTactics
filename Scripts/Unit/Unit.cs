using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/18/2025]
 * [Unit Main Script
 * NOTE: all of this is one script for now,
 * but will likely need to be split up into different
 * scripts. comments of node type will likely be where they split]
 */

public partial class Unit : Node2D
{
    //maybe somewhere else, but will likely leave in this script
    private Vector2 _cell = Vector2.Zero;
    private bool _isSelected = false;

    [Export] private UnitPath _unitPath;
    private bool _unitCanWalk = false;

    [Export] private UnitSprite _unitSprite;

    [Export] private UnitStats _unitStats;

    //animation player, but i might need to make a state machine
    //will keep in here for now to see how is selected works
    [Export] private AnimationPlayer _animPlayer; 

    //map
    [Export] private GridResource _grid;

    public override void _Ready()
    {
        this.cell = _grid.CalculateGridCoordinates(Position);
        Position = _grid.CalculateMapPosition(cell);

        //test
        Vector2[] test = new Vector2[4];
        test[0] = new Vector2(1, 1);
        test[1] = new Vector2(1, 2);
        test[2] = new Vector2(2, 3);
        test[3] = new Vector2(3, 0);

        _unitPath.SetWalkPath(test, _grid);
    }

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;

        if (_unitPath != null && _unitCanWalk)
        {
            _unitPath.WalkUnit(fDelta, _grid, cell);
        }
    }

    //--- PROPERTIES ---
    public Vector2 cell
    {
        set
        {
            if (_grid != null)
            {
                _cell = _grid.Clamp(value);
            }
        }
        get { return _cell; }
    }

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
}
