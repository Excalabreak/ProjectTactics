using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/05/2025]
 * [Manages any change in direction]
 */

public partial class UnitDirection : Node2D
{
    private DirectionEnum _currentFacing = DirectionEnum.UP;
    public Action<DirectionEnum> UpdateCurrentFacing;

    public override void _Ready()
    {
        //this seems like it can be a problem later
        UpdateCurrentFacing?.Invoke(_currentFacing);
    }

    public DirectionEnum currentFacing
    {
        get { return _currentFacing; }
        set
        {
            _currentFacing = value;
            UpdateCurrentFacing?.Invoke(_currentFacing);
        }
    }
}
