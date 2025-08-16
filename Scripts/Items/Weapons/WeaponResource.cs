using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/15/2025]
 * [resource for weapon]
 */

[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] private string _weaponName;
    [Export(PropertyHint.MultilineText)] private string _description;

    //make enum for damage type
    [Export] private Dictionary<DamageType, int> _damage;

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
}
