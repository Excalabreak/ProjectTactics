using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/29/2025]
 * [class for unit's current stats]
 */

public partial class UnitStats : Node
{
    [Export] private Unit _unit;
    [Export] private BaseStats _baseStats;
    //temp, make pretty later

    [Export] private Label _healthLable;
    private int _maxHP = 10;
    private int _currentHP = 10;

    /// <summary>
    /// makes sure stats has all enums
    /// </summary>
    public override void _Ready()
    {
        _maxHP = GetStat(UnitStatEnum.HEALTH);
        _currentHP = _maxHP;
        UpdateHealthUI();
    }

    /// <summary>
    /// damages the unit and checks for death
    /// NOTE: very simple now
    /// </summary>
    /// <param name="damage"></param>
    public void DamageUnit(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0)
        {
            UnitEventManager.OnUnitDeathEvent(_unit);
            _unit.QueueFree();
        }
        UpdateHealthUI();
    }

    /// <summary>
    /// updates the UI for 
    /// </summary>
    public void UpdateHealthUI()
    {
        _healthLable.Text = _currentHP + "/" + _maxHP;
    }

    /// <summary>
    /// gets the base stats of a unit
    /// </summary>
    /// <param name="stat">which stat</param>
    /// <returns>base stat</returns>
    public int GetStat(UnitStatEnum stat)
    {
        int output = 0;
        //i do not like this
        //would want an event system that can return
        //all the needed stats and add them together
        output += _baseStats.GetBaseStat(stat);
        return output;
    }

    public int maxHP
    {
        get { return _maxHP; }
    }

    public int currentHP
    {
        get { return _currentHP; }
    }
}
