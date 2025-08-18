using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/17/2025]
 * [resource for weapon]
 */

[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] private string _weaponName;
    [Export(PropertyHint.MultilineText)] private string _description;

    //make enum for damage type
    [Export] private DamageType _damageType;
    [Export] private int _damage;

    [Export] private int _minRange;
    [Export] private int _maxRange;

    [Export] private int _handling;
    [Export] private int _critChance;

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

    public string weaponName
    {
        get { return _weaponName; }
    }

    public string description
    {
        get { return _description; }
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

    public int weight
    {
        get { return _weight; }
    }

    public int size
    {
        get { return _size; }
    }
}
