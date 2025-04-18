using Godot;
using System;

public partial class UnitStats : Node
{
    //might need to make this a resource
    [Export] private int _moveRange = 6;

    public int moveRange
    {
        get { return _moveRange; }
        set { _moveRange = value; }
    }
}
