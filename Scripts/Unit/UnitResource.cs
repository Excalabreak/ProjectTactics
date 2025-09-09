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
    [Export] private string _equiptWeapon;
    //WARNING: interfaces can't be exported,
    //using weapon resources for testing
    //(maybe make a db for items and store id)
    [Export] private string[] _inventoryWeapons;
    [Export] private string[] _consumableItems;

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
        get { return WeaponDataBase.Instance.GetItem(_equiptWeapon); }
    }

    public WeaponResource[] inventoryWeapons
    {
        get 
        {
            WeaponResource[] storedWeapons = new WeaponResource[_inventoryWeapons.Length];

            for (int i = 0; i < _inventoryWeapons.Length; i++)
            {
                storedWeapons[i] = WeaponDataBase.Instance.GetItem(_inventoryWeapons[i]);
            }
             
            return storedWeapons; 
        }
    }
    public ConsumableItemResource[] consumableItems
    {
        get
        {
            ConsumableItemResource[] storedConsumables = new ConsumableItemResource[_consumableItems.Length];

            for (int i = 0; i < _inventoryWeapons.Length; i++)
            {
                storedConsumables[i] = ConsumableDataBase.Instance.GetItem(_consumableItems[i]);
            }

            return storedConsumables;
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
