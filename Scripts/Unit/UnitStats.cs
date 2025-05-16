using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/16/2025]
 * [class for unit stats]
 */

public partial class UnitStats : Node
{
    //might need to make this a resource
    [Export] private int _health = 10;
    [Export] private int _mana = 10;
    [Export] private int _strength = 5;
    [Export] private int _magic = 5;
    [Export] private int _speed = 5;
    [Export] private int _dexterity = 5;
    [Export] private int _defense = 5;
    [Export] private int _resistance = 5;
    [Export] private float _moveRange = 6;
    [Export] private float _visionRange = 8;

    //simple property
    public int health
    {
        get { return _health; }
        set { _health = value; }
    }

    public int mana
    {
        get { return _mana; }
        set { _mana = value; }
    }

    public int strength
    {
        get { return _strength; }
        set { _strength = value; }
    }

    public int magic
    {
        get { return _magic; }
        set { _magic = value; }
    }

    public int speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public int dexterity
    {
        get { return _dexterity; }
        set { _dexterity = value; }
    }

    public int defense
    {
        get { return _defense; }
        set { _defense = value; }
    }

    public int resistance
    {
        get { return _resistance; }
        set { _resistance = value; }
    }

    public float moveRange
    {
        get { return _moveRange; }
        set { _moveRange = value; }
    }

    public float visionRange
    {
        get { return _visionRange; }
        set { _visionRange = value; }
    }
}
