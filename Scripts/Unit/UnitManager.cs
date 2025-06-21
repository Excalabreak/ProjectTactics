using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/02/2025]
 * [manages all units and groups and ]
 */

public partial class UnitManager : Node2D
{
    private Dictionary<UnitGroupEnum, UnitGroup> _unitGroups = new Dictionary<UnitGroupEnum, UnitGroup>();

    public override void _Ready()
    {
        foreach (Node2D child in GetChildren())
        {
            UnitGroup unit = child as UnitGroup;
            if (unit == null)
            {
                continue;
            }

            _unitGroups.Add(unit.group, unit);
        }
    }

    /// <summary>
    /// gets all units in array
    /// </summary>
    /// <returns>array of all units</returns>
    public Unit[] GetAllUnits()
    {
        List<Unit> units = new List<Unit>();

        foreach (KeyValuePair<UnitGroupEnum, UnitGroup> unitGroup in _unitGroups)
        {
            units.AddRange(_unitGroups[unitGroup.Key].GetUnitGroup());
        }

        return units.ToArray();
    }

    /// <summary>
    /// gets the enums of the groups
    /// </summary>
    /// <returns>array of UnitGroupEnum sorted</returns>
    public UnitGroupEnum[] GetAllUnitGroupEnums()
    {
        List<UnitGroupEnum> groups = new List<UnitGroupEnum>();

        foreach (KeyValuePair<UnitGroupEnum, UnitGroup> unitGroup in _unitGroups)
        {
            groups.Add(unitGroup.Key);
        }

        groups.Sort();

        return groups.ToArray();
    }

    /// <summary>
    /// returns weather or not a unit from movingGroup can pass
    /// a unit from standingGroup
    /// </summary>
    /// <param name="movingGroup">unit trying to pass</param>
    /// <param name="standingGroup">unit that is standing</param>
    /// <returns>true if moving group can pass standing group</returns>
    public bool CanPass(UnitGroupEnum movingGroup, UnitGroupEnum standingGroup)
    {
        return _unitGroups[movingGroup].CanPass(standingGroup);
    }

    /// <summary>
    /// returns weather or not a unit from attackingGroup can attack
    /// a unit from defendingGroup
    /// </summary>
    /// <param name="attackingGroup">unit that initiates combat</param>
    /// <param name="defendingGroup">unit that is being attacked</param>
    /// <returns>true if the units can fight</returns>
    public bool CanAttack(UnitGroupEnum attackingGroup, UnitGroupEnum defendingGroup)
    {
        return _unitGroups[attackingGroup].CanAttack(defendingGroup);
    }
}
