using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/25/2025]
 * [holds information about the units in a group]
 */

public partial class UnitGroup : Node2D
{
    [Export] private UnitGroupEnum _group;
    [Export] private Godot.Collections.Array<UnitGroupEnum> _passingGroup;
    [Export] private Godot.Collections.Array<UnitGroupEnum> _attackingGroup;

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
    public Unit[] GetUnits()
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

    /// <summary>
    /// returns weather or not a group can attack this group
    /// </summary>
    /// <param name="group">other unit group</param>
    /// <returns>true if they can attack</returns>
    public bool CanAttack(UnitGroupEnum group)
    {
        return (group != _group && _attackingGroup.Contains(group));
    }

    // properties

    public UnitGroupEnum group
    {
        get { return _group; }
    }
}
