using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/29/2025]
 * [manages all units and groups and ]
 */

public partial class UnitManager : Node2D
{
    private List<UnitGroup> _unitGroups = new List<UnitGroup>();

    public override void _Ready()
    {
        SetUnitGroupList();
    }

    /// <summary>
    /// gets all unit groups
    /// </summary>
    private void SetUnitGroupList()
    {
        foreach (Node2D child in GetChildren())
        {
            var unit = child as UnitGroup;
            if (unit == null)
            {
                continue;
            }
            _unitGroups.Add(unit);
        }
    }

    public Unit[] GetAllUnits()
    {
        List<Unit> units = new List<Unit>();

        foreach (UnitGroup unitGroup in _unitGroups)
        {
            units.AddRange(unitGroup.GetUnitGroup());
        }

        return units.ToArray();
    }
}
