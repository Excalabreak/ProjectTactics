using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [displays stats for the player]
 */

public partial class UIStats : Control
{
    [Export] private Label _HPLable;
    [Export] private Label _MOVLable;
    [Export] private Label _ATKLable;
    [Export] private Label _DEFLable;
    [Export] private Label _RNGLable;

    /// <summary>
    /// displays the stats of a unit
    /// </summary>
    /// <param name="unit"></param>
    public void ShowUnitStats(Unit unit)
    {
        _HPLable.Text = "HP: " + unit.unitStats.currentHP + "/" + unit.unitStats.maxHP;
        _MOVLable.Text = "MOV: " + unit.unitActionEconomy.currentMove + "/" + unit.unitActionEconomy.maxMove;
        _ATKLable.Text = "ATK: " + unit.unitStats.GetBaseStat(UnitStatEnum.STRENGTH);
        _DEFLable.Text = "DEF: " + unit.unitStats.GetBaseStat(UnitStatEnum.DEFENSE);
        _RNGLable.Text = "RANGE: " + unit.attackRange;

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
