using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/15/2025]
 * [handles inventory for units]
 */

public partial class UnitInventory : Node
{
    [Export] private string _currentWeapon;

    public string currentWeapon
    {
        get { return _currentWeapon; }
    }
}
