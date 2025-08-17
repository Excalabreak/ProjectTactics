using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/16/2025]
 * [a unit resource that holds unit data
 * for the unit to load onto their scripts
 * 
 * THIS IS MEANT FOR LEVEL DESIGN,
 * NEED TO FIND A BETTER WAY OF GOING ABOUT THIS]
 */

[GlobalClass]
public partial class UnitResource : Resource
{
    [ExportGroup("Sprites")]
    [Export] private Texture2D _sprite;
    [Export] private Vector2 _spriteOffset = Vector2.Zero;

    [ExportGroup("Stats")]
    [Export] private Dictionary<UnitStatEnum, int> _baseStats = new Dictionary<UnitStatEnum, int>();

    [ExportGroup("Inventory")]
    [Export] private WeaponResource _equiptWeapon;

    [ExportGroup("AttackRange")]
    [Export] private int _attackRange = 1;

    [ExportGroup("Direction")]
    [Export] private DirectionEnum _startingDirection = DirectionEnum.UP;

    [ExportGroup("IsCommander")]
    [Export] private bool _isCommander = false;

    public Texture2D sprite
    {
        get { return _sprite; }
    }

    public WeaponResource equiptWeapon
    {
        get { return _equiptWeapon; }
    }

    public Vector2 spriteOffset
    {
        get { return _spriteOffset; }
    }

    public Dictionary<UnitStatEnum, int> baseStats
    {
        get { return _baseStats; }
    }

    public int attackRange
    {
        get { return _attackRange; }
    }

    public DirectionEnum startingDirection
    {
        get { return _startingDirection; }
    }

    public bool isCommander
    {
        get { return _isCommander; }
    }
}
