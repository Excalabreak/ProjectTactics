using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/22/2025]
 * [class for unit's current stats]
 */

public partial class UnitStats : Node
{
    [Export] private Unit _unit;
    [Export] private BaseStats _baseStats;
    [Export] private UnitInventory _unitInventory;
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

    /// <summary>
    /// the attack of the unit 
    /// (aka damage if unit has no buffs and opponent has no def/res)
    /// </summary>
    public int attack
    {
        get
        {
            if (_unitInventory.equiptWeapon.IsPhysical())
            {
                return _baseStats.GetBaseStat(UnitStatEnum.STRENGTH) + _unitInventory.equiptWeapon.damage;
            }
            return _baseStats.GetBaseStat(UnitStatEnum.MAGIC) + _unitInventory.equiptWeapon.damage;
        }
    }

    /// <summary>
    /// base hit rate of hitting unit
    /// </summary>
    public int hitRate
    {
        get
        {
            return _unitInventory.equiptWeapon.handling + (_baseStats.GetBaseStat(UnitStatEnum.DEXTERITY) * 3 / 2);
        }
    }

    /// <summary>
    /// base hit rate of hitting unit
    /// </summary>
    public int critRate
    {
        get
        {
            return _unitInventory.equiptWeapon.critChance + (_baseStats.GetBaseStat(UnitStatEnum.DEXTERITY) / 2);
        }
    }

    /// <summary>
    /// how much to take off of hit rate
    /// </summary>
    public int avoid
    {
        get
        {
            return _baseStats.GetBaseStat(UnitStatEnum.SPEED) * 3 / 2;
        }
    }

    /// <summary>
    /// how much physical damage gets removed
    /// </summary>
    public int protection
    {
        get
        {
            return _baseStats.GetBaseStat(UnitStatEnum.DEFENSE);
        }
    }

    /// <summary>
    /// how much magical damage gets removed
    /// </summary>
    public int resilience
    {
        get
        {
            return _baseStats.GetBaseStat(UnitStatEnum.RESISTANCE);
        }
    }

    //simple getters

    public int maxHP
    {
        get { return _maxHP; }
    }

    public int currentHP
    {
        get { return _currentHP; }
    }
}
