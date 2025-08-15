using Godot;
using System;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/01/2025]
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

        UnitStats attackingStats = initUnit.unitStats;
        UnitStats defendingStats = targetUnit.unitStats;
        Vector2 targetPos = targetUnit.cell;

        BattleDamage(attackingStats, defendingStats);

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
            BattleDamage(defendingStats, attackingStats);
        }
    }

    /// <summary>
    /// calls to damage unit
    /// </summary>
    /// <param name="attackingUnit">unit that is attacking</param>
    /// <param name="defendingUnit">unit that is defending </param>
    private void BattleDamage(UnitStats attackingUnit, UnitStats defendingUnit)
    {
        defendingUnit.DamageUnit(attackingUnit.GetStat(UnitStatEnum.STRENGTH) - defendingUnit.GetStat(UnitStatEnum.DEFENSE));
    }
}
