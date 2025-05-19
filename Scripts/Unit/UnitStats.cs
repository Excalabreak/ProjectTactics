using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/19/2025]
 * [class for unit stats]
 */

public partial class UnitStats : Node
{
    [Export] private Dictionary<UnitStatEnum, int> _baseStats = new Dictionary<UnitStatEnum, int>();

    /// <summary>
    /// makes sure stats has all enums
    /// </summary>
    public override void _Ready()
    {
        foreach (UnitStatEnum stat in Enum.GetValues(typeof(DirectionEnum)))
        {
            if (_baseStats.ContainsKey(stat))
            {
                continue;
            }

            _baseStats.Add(stat, 1);
        }
    }

    /// <summary>
    /// gets the base stats of a unit
    /// </summary>
    /// <param name="stat">which stat</param>
    /// <returns>base stat</returns>
    public int GetBaseStat(UnitStatEnum stat)
    {
        return _baseStats[stat];
    }
}
