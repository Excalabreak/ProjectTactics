using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/16/2025]
 * [handles inventory for units]
 */

public partial class UnitInventory : Node
{
    [Export] private Unit _unit;
    private WeaponResource _equiptWeapon;

    public override void _Ready()
    {
        _equiptWeapon = _unit.unitResource.equiptWeapon;
    }

    public WeaponResource equiptWeapon
    {
        get { return _equiptWeapon; }
    }
}
