using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
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
    [Export] private UnitClassResource _unitClass;
    [Export] private Dictionary<UnitStatEnum, int> _baseStats = new Dictionary<UnitStatEnum, int>();

    [ExportGroup("Inventory")]
    [Export] private WeaponResource _equiptWeapon;
    //WARNING: interfaces can't be exported,
    //using weapon resources for testing
    //(maybe make a db for items and store id)
    [Export] private WeaponResource[] _inventoryItems;
    [Export] private ConsumableItemResource[] _consumableItems;

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

    public WeaponResource[] inventoryItems
    {
        get 
        {
            return _inventoryItems; 
        }
    }
    public ConsumableItemResource[] consumableItems
    {
        get
        {
            return _consumableItems;
        }
    }

    public Vector2 spriteOffset
    {
        get { return _spriteOffset; }
    }

    public UnitClassResource unitClass
    {
        get { return _unitClass; }
    }

    public Dictionary<UnitStatEnum, int> baseStats
    {
        get { return _baseStats; }
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
