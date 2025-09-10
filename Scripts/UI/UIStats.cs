using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/10/2025]
 * [displays stats for the player]
 */

public partial class UIStats : Control
{
    [Export] private Label _statsLable;
    [Export] private Label _combatStatsLable;

    /// <summary>
    /// displays the stats of a unit
    /// </summary>
    /// <param name="unit"></param>
    public void ShowUnitStats(Unit unit)
    {
        _statsLable.Text = GetStatsText(unit);
        _combatStatsLable.Text = GetCombatStatsText(unit);

        this.Visible = true;
    }

    /// <summary>
    /// returns a string for stats
    /// </summary>
    /// <param name="unit">unit for stats</param>
    /// <returns>string for stats</returns>
    private string GetStatsText(Unit unit)
    {
        UnitStats stats = unit.unitStats;
        string statsText = statsText = "HP: " + stats.currentHP + "/" + unit.unitStats.maxHP;

        statsText += "\nSTR: " + stats.GetStat(UnitStatEnum.STRENGTH);
        statsText += "\nMAG: " + stats.GetStat(UnitStatEnum.MAGIC);
        statsText += "\nDEX: " + stats.GetStat(UnitStatEnum.DEXTERITY);
        statsText += "\nSPD: " + stats.GetStat(UnitStatEnum.SPEED);
        statsText += "\nDEF: " + stats.GetStat(UnitStatEnum.DEFENSE);
        statsText += "\nRES: " + stats.GetStat(UnitStatEnum.RESISTANCE);

        statsText += "\n\nMOV: " + unit.unitActionEconomy.currentMove + "/" + unit.unitActionEconomy.maxMove;

        if (unit.unitInventory.equiptWeapon.minRange == unit.unitInventory.equiptWeapon.maxRange)
        {
            statsText += "\nRNG: " + unit.unitInventory.equiptWeapon.maxRange;
        }
        else
        {
            statsText += "\nRNG: " + unit.unitInventory.equiptWeapon.minRange + " - " + unit.unitInventory.equiptWeapon.maxRange;
        }

        statsText += "\nVIS: " + stats.GetStat(UnitStatEnum.VISION);

        return statsText;
    }

    /// <summary>
    /// returns a string for combat stats
    /// </summary>
    /// <param name="unit">unit for stats</param>
    /// <returns>string for combat stats</returns>
    private string GetCombatStatsText(Unit unit)
    {
        UnitStats stats = unit.unitStats;

        string statsText = "ATK: " + stats.attack;
        statsText += "\n\nACC: " + stats.accuracy;
        statsText += "\nAVO: " + stats.avoid;
        statsText += "\n\nCRT RATE: " + stats.critRate;
        statsText += "\nCRT MOD: " + unit.unitInventory.equiptWeapon.critModifyer;
        statsText += "\n\nPRT: " + stats.protection;
        statsText += "\nRIS: " + stats.resilience;

        return statsText;

    }

    /// <summary>
    /// hides the stats pannel
    /// </summary>
    public void HideStatsPanel()
    {
        this.Visible = false;
    }
}
