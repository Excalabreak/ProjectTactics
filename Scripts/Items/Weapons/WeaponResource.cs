using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/07/2025]
 * [resource for weapon]
 */

[GlobalClass]
public partial class WeaponResource : Resource, IInventoryItem, IEquipable
{
    [Export] private string _devName;
    [Export] private string _weaponName;
    [Export(PropertyHint.MultilineText)] private string _description;

    //make enum for damage type
    [Export] private WeaponTypeEnum _weaponType;
    [Export] private DamageType _damageType;
    [Export] private bool _canUseSpells;
    [Export] private int _damage;

    [Export] private int _minRange;
    [Export] private int _maxRange;

    [Export] private int _handling;
    [Export] private int _critChance;
    [Export] private float _critModifyer;

    [Export] private int _weight;

    [Export] private int _size;

    //for special menu action
    //[Export] private SpecialWeaponAction _specialAction

    //for passive buffs
    //[Export] private ItemPassiveBuff _passiveBuff

    /// <summary>
    /// checks if the damage type is physical or magical
    /// </summary>
    /// <returns>true if the weapon is physical</returns>
    public bool IsPhysical()
    {
        return _damageType != DamageType.MAGIC;
    }

    public string devName
    {
        get { return _devName; }
    }

    public string itemName
    {
        get { return _weaponName; }
    }

    public string description
    {
        get 
        {
            string desc = _description;
            desc += "\nDamage: " + _damage;
            desc += "\nHandling: " + _handling;
            desc += "\nCrit Chance: " + _critChance;
            desc += "\nCrit Mod: " + _critModifyer;

            if (_minRange == _maxRange)
            {
                desc += "\nRange: " + _minRange;
            }
            else
            {
                desc += "\nRange: " + _minRange + " - " + _maxRange;
            }

            desc += "\nInv. Size: " + _size;
            return desc; 
        }
    }

    public WeaponTypeEnum weaponType
    {
        get { return _weaponType; }
    }

    public DamageType damageType
    {
        get { return _damageType; }
    }

    public int damage
    {
        get { return _damage; }
    }

    public int minRange
    {
        get { return _minRange; }
    }

    public int maxRange
    {
        get { return _maxRange; }
    }

    public int handling
    {
        get { return _handling; }
    }

    public int critChance
    {
        get { return _critChance; }
    }

    public float critModifyer
    {
        get { return _critModifyer; }
    }

    public int weight
    {
        get { return _weight; }
    }

    public int size
    {
        get { return _size; }
    }

    public override string ToString()
    {
        return _weaponName;
    }

    public EquipableSlotEnum equipableSlot
    {
        get { return EquipableSlotEnum.WEAPON; }
    }
}
