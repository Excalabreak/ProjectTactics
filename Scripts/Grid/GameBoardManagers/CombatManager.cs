using Godot;
using System;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/17/2025]
 * [manages combat]
 */

public partial class CombatManager : Node
{
    [Export] private GameBoard _gameBoard;

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

        BattleDamage(initUnit, targetUnit);

        //counter attack
        //temp, there is a faster way of doing this
        //figure out later
        if (!_gameBoard.IsOccupied(targetPos))
        {
            return;
        }
        Vector2[] opposingAttackableCells = _gameBoard.FloodFill(targetUnit.cell, targetUnit.attackRange);
        if (opposingAttackableCells.Contains(initUnit.cell))
        {
            BattleDamage(targetUnit, initUnit);
        }
    }

    /// <summary>
    /// calls to damage unit
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending </param>
    private void BattleDamage(Unit attackingUnit, Unit defendingUnit)
    {
        defendingUnit.unitStats.DamageUnit(CalculateDamage(attackingUnit, defendingUnit));
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
        UnitStatEnum attackerStat = UnitStatEnum.STRENGTH;
        UnitStatEnum defenderStat = UnitStatEnum.DEFENSE;

        bool skipWeaponCheck = false;
        int weaponDamage = 0;

        if (attackingUnit.unitInventory.equiptWeapon == null)
        {
            skipWeaponCheck = true;
        }
        else
        {
            weaponDamage = attackingUnit.unitInventory.equiptWeapon.damage; 
        }

        if (!skipWeaponCheck && !attackingUnit.unitInventory.equiptWeapon.IsPhysical())
        {
            attackerStat = UnitStatEnum.MAGIC;
            defenderStat = UnitStatEnum.RESISTANCE;
        }

        return (attackingUnit.unitStats.GetStat(attackerStat) + weaponDamage) 
            - defendingUnit.unitStats.GetStat(defenderStat);
    }
}
