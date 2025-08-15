using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/15/2025]
 * [holds base stats for units]
 */

public partial class BaseStats : Node
{
    [Export] private Unit _unit;
    private Dictionary<UnitStatEnum, int> _baseStats = new Dictionary<UnitStatEnum, int>();

    public override void _Ready()
    {
        _baseStats = _unit.unitResource.baseStats;

        foreach (UnitStatEnum stat in Enum.GetValues(typeof(DirectionEnum)))
        {
            if (_baseStats.ContainsKey(stat))
            {
                continue;
            }

            _baseStats.Add(stat, 0);
        }
    }

    /// <summary>
    /// gets the base stats of a unit
    /// </summary>
    /// <param name="stat">which stat</param>
    /// <returns>base stat</returns>
    public int GetBaseStat(UnitStatEnum stat)
    {
        if (!_baseStats.ContainsKey(stat))
        {
            GD.Print("can't get base stats from: " + _unit.Name);
            return 0;
        }
        return _baseStats[stat];
    }
}
