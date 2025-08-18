using Godot;
using System;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/18/2025]
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

        if (CanReach(targetUnit, initUnit))
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
        int hitChance = GD.RandRange(1,100);

        if (CalculateHitRate(attackingUnit,defendingUnit) >= hitChance)
        {
            defendingUnit.unitStats.DamageUnit(CalculateDamage(attackingUnit, defendingUnit));
        }
        else
        {
            GD.Print(attackingUnit.Name + " MISS");
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

        return Mathf.Clamp(attackingUnit.unitStats.hitRate - defendingUnit.unitStats.avoid, 0, 100);
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
