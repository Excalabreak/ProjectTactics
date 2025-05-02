using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/02/2025]
 * [holds information about the units in a group]
 */

public partial class UnitGroup : Node2D
{
    [Export] private UnitGroupEnum _group;
    [Export] private Godot.Collections.Array<UnitGroupEnum> _passingGroup;

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

            unit.unitGroup = _group;
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

    /// <summary>
    /// returns weather or not the group can pass this group
    /// </summary>
    /// <param name="group">other unit group</param>
    /// <returns>true if can pass</returns>
    public bool CanPass(UnitGroupEnum group)
    {
        return (group == _group || _passingGroup.Contains(group));
    }

    // properties

    public UnitGroupEnum group
    {
        get { return _group; }
    }
}
