using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/17/2025]
 * [displays stats for the player]
 */

public partial class UIStats : Control
{
    [Export] private Label _statsLable;

    /// <summary>
    /// displays the stats of a unit
    /// </summary>
    /// <param name="unit"></param>
    public void ShowUnitStats(Unit unit)
    {
        string statsText = statsText = "HP: " + unit.unitStats.currentHP + "/" + unit.unitStats.maxHP;

        statsText += "\nSTR: " + unit.unitStats.GetStat(UnitStatEnum.STRENGTH);
        statsText += "\nMAG: " + unit.unitStats.GetStat(UnitStatEnum.MAGIC);
        statsText += "\nDEX: " + unit.unitStats.GetStat(UnitStatEnum.DEXTERITY);
        statsText += "\nSPD: " + unit.unitStats.GetStat(UnitStatEnum.SPEED);
        statsText += "\nDEF: " + unit.unitStats.GetStat(UnitStatEnum.DEFENSE);
        statsText += "\nRES: " + unit.unitStats.GetStat(UnitStatEnum.RESISTANCE);

        statsText += "\n\nMOV: " + unit.unitActionEconomy.currentMove + "/" + unit.unitActionEconomy.maxMove;

        if (unit.unitInventory.equiptWeapon.minRange == unit.unitInventory.equiptWeapon.maxRange)
        {
            statsText += "\nRNG: " + unit.unitInventory.equiptWeapon.maxRange;
        }
        else
        {
            statsText += "\nRNG: " + unit.unitInventory.equiptWeapon.minRange + " - " + unit.unitInventory.equiptWeapon.maxRange;
        }

        statsText += "\nVIS: " + unit.unitStats.GetStat(UnitStatEnum.VISION);

        _statsLable.Text = statsText;

        this.Visible = true;
    }

    /// <summary>
    /// hides the stats pannel
    /// </summary>
    public void HideStatsPanel()
    {
        this.Visible = false;
    }
}
