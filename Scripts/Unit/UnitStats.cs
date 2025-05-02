using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/18/2025]
 * [class for unit stats]
 */

public partial class UnitStats : Node
{
    //might need to make this a resource
    [Export] private float _moveRange = 6;

    //simple property
    public float moveRange
    {
        get { return _moveRange; }
        set { _moveRange = value; }
    }
}
