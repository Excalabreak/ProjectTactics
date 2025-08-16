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
    private List<WeaponResource> _equiptWeapons;

    public override void _Ready()
    {
        _equiptWeapons.AddRange(_unit.unitResource.equiptWeapons);
    }

    public WeaponResource[] equiptWeapons
    {
        get { return _equiptWeapons.ToArray(); }
    }
}
