using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/29/2025]
 * [holds information about the units in a group]
 */

public partial class UnitGroup : Node2D
{
    private List<Unit> _units = new List<Unit>();

    /// <summary>
    /// adds all the children as units
    /// </summary>
    public override void _Ready()
    {
        SetUnitList();
    }

    /// <summary>
    /// sets up unit list
    /// </summary>
    private void SetUnitList()
    {
        foreach (Node2D child in GetChildren())
        {
            var unit = child as Unit;
            if (unit == null)
            {
                continue;
            }
            _units.Add(unit);
        }
    }

    /// <summary>
    /// returns an array of units
    /// </summary>
    /// <returns></returns>
    public Unit[] GetUnitGroup()
    {
        return _units.ToArray();
    }
}
