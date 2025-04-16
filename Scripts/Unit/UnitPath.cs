using Godot;
using System;
using System.IO;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/16/2025]
 * [unit's path it fallows when moving]
 */

public partial class UnitPath : Path2D
{
    [Export] private Unit _unit;

    //path2d
    [Signal] public delegate void WalkFinishedEventHandler();
    [Export] private PathFollow2D _pathFollow;
    private bool _isWalking = false;

    //might need to put in a settings
    [Export] private float _moveSpeed = 600f;

    public override void _Ready()
    {
        Curve = new Curve2D();
    }

    //note, grid might need to be taken out and put in a map manager
    public void WalkUnit(float delta, GridResource grid, Vector2 cell)
    {
        _pathFollow.Progress += _moveSpeed * delta;

        if (_pathFollow.ProgressRatio >= 1f)
        {
            this._isWalking = false;
            _pathFollow.Progress = 0f;
            _unit.Position = grid.CalculateMapPosition(cell);
            Curve.ClearPoints();
            EmitSignal("WalkFinished");
        }
    }

    //note, grid might need to be taken out and put in a map manager
    public void SetWalkPath(Vector2[] path, GridResource grid)
    {
        if (path.Length <= 0)
        {
            return;
        }
        if (Curve == null)
        {
            Curve = new Curve2D();
        }

        Curve.AddPoint(Vector2.Zero);
        foreach (Vector2 point in path)
        {
            Curve.AddPoint(grid.CalculateMapPosition(point) - _unit.Position);
        }
        _unit.cell = path[path.Length - 1];
        isWalking = true;
    }

    public bool isWalking
    {
        set
        {
            _isWalking = value;
            _unit.unitCanWalk = value;
        }
        get { return _isWalking; }
    }
}
