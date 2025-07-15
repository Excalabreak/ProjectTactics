using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/15/2025]
 * [class for unit stats]
 */

public partial class UnitStats : Node
{
    [Export] private Unit _unit;
    [Export] private Dictionary<UnitStatEnum, int> _baseStats = new Dictionary<UnitStatEnum, int>();
    //temp, make pretty later

    [Export] private Label _healthLable;
    private int _maxHP = 10;
    private int _currentHP = 10;

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

        _maxHP = _baseStats[UnitStatEnum.HEALTH];
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
    public int GetBaseStat(UnitStatEnum stat)
    {
        return _baseStats[stat];
    }

    /// <summary>
    /// adds the amount to stat
    /// to subtract, just use negative
    /// </summary>
    /// <param name="stat">stat to change</param>
    /// <param name="amount">amount to add</param>
    public void AddToStat(UnitStatEnum stat, int amount)
    {
        _baseStats[stat] = _baseStats[stat] + amount;
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
