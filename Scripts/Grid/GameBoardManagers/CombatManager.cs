using Godot;
using System;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/22/2025]
 * [manages combat]
 */

public partial class CombatManager : Node
{
    [Export] private GameBoard _gameBoard;
    [Export] private UIManager _uiManager;

    /// <summary>
    /// calls to calculate unit combat
    /// </summary>
    /// <param name="initUnit">unit initiating comabat</param>
    /// <param name="targetUnit">unit targeted</param>
    public void UnitCombat(Unit initUnit, Unit targetUnit)
    {
        //makes sure units are facing the right direction
        //will likely need to change when engaged in combat is added
        DirectionEnum[] initUnitPossibleDirection = DirectionManager.Instance.GetClosestDirection(initUnit.cell, targetUnit.cell);
        if (!initUnitPossibleDirection.Contains(initUnit.unitDirection.currentFacing))
        {
            initUnit.unitDirection.currentFacing = initUnitPossibleDirection[0];
        }

        targetUnit.unitDirection.currentFacing = DirectionManager.Instance.GetOppositeDirection(initUnit.unitDirection.currentFacing);

        initUnit.unitActionEconomy.UseAction();

        Vector2 targetPos = targetUnit.cell;

        UnitAttack(initUnit, targetUnit);

        //counter attack
        //temp, there is a faster way of doing this
        //figure out later
        if (!_gameBoard.IsOccupied(targetPos))
        {
            return;
        }

        if (CanReach(targetUnit, initUnit))
        {
            UnitAttack(targetUnit, initUnit);
        }
    }

    /// <summary>
    /// calls to damage unit
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending </param>
    private void UnitAttack(Unit attackingUnit, Unit defendingUnit)
    {
        int hitChance = GD.RandRange(1,100);
        float critMod = 1;

        if (CalculateHitRate(attackingUnit,defendingUnit) >= hitChance)
        {
            hitChance = GD.RandRange(1, 100);

            if (CalculateCritRate(attackingUnit, defendingUnit) >= hitChance)
            {
                critMod = attackingUnit.unitInventory.equiptWeapon.critModifyer;
                _uiManager.AddToBattleLog(attackingUnit.Name + " CRIT!!!");
            }
            int damage = Mathf.RoundToInt((float)CalculateDamage(attackingUnit, defendingUnit) * critMod);

            _uiManager.AddToBattleLog(attackingUnit.Name + " hits " + defendingUnit.Name + " for " + damage + " damage.");
            defendingUnit.unitStats.DamageUnit(damage);
        }
        else
        {
            _uiManager.AddToBattleLog(attackingUnit.Name + " MISSED!");
        }
    }

    /// <summary>
    /// calculates how much damage should be done if the attack goes through
    /// passes units so that it can account for equiptment
    /// </summary>
    /// <param name="attackingUnit">attacking unit</param>
    /// <param name="defendingUnit">defending unit</param>
    /// <returns>damage</returns>
    public int CalculateDamage(Unit attackingUnit, Unit defendingUnit)
    {
        if (attackingUnit.unitInventory.equiptWeapon.IsPhysical())
        {
            return attackingUnit.unitStats.attack - defendingUnit.unitStats.protection;
        }
        

        return attackingUnit.unitStats.attack - defendingUnit.unitStats.resilience;
    }

    /// <summary>
    /// calculates the chances of a unit's attack hitting
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending</param>
    /// <returns>percent chance that a unit hits</returns>
    public int CalculateHitRate(Unit attackingUnit, Unit defendingUnit)
    {
        if (!CanReach(attackingUnit, defendingUnit))
        {
            return 0;
        }

        return Mathf.Clamp(attackingUnit.unitStats.accuracy - defendingUnit.unitStats.avoid, 0, 100);
    }

    /// <summary>
    /// calculates the chances of a unit critting
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending</param>
    /// <returns>percent chance that a unit crits</returns>
    public int CalculateCritRate(Unit attackingUnit, Unit defendingUnit)
    {
        if (!CanReach(attackingUnit, defendingUnit))
        {
            return 0;
        }

        return Mathf.Clamp(attackingUnit.unitStats.critRate - (defendingUnit.unitStats.critRate / 2), 0, 100);
    }

    /// <summary>
    /// returns true if unit can hit based on range
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending</param>
    /// <returns>unit is in range of unit</returns>
    public bool CanReach(Unit attackingUnit, Unit defendingUnit)
    {
        Vector2 attackCell = attackingUnit.cell;
        Vector2 defendCell = defendingUnit.cell;

        int dist = Mathf.RoundToInt(Mathf.Abs(defendCell.X - attackCell.X) + Mathf.Abs(defendCell.Y - attackCell.Y));

        return (attackingUnit.unitInventory.equiptWeapon.minRange <= dist
            && attackingUnit.unitInventory.equiptWeapon.maxRange >= dist);
    }
}
