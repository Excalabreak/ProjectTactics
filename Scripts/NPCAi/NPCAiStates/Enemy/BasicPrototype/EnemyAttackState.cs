using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/22/2025]
 * [attack state for enemy ai
 * NOTE: very basic for now]
 */

public partial class EnemyAttackState : NPCAiState
{
    private bool isWalking = false;
    /// <summary>
    /// finds the closest unit and goes for
    /// attacks
    /// </summary>
    public override async void TurnLogic()
    {
        Vector2 targetLoc = stateMachine.gameBoard.ClosestUnitPosition(UnitGroupEnum.PLAYER, stateMachine.unit.cell);
        
        Vector2[] attackableArea = stateMachine.gameBoard.RangeFloodFill(stateMachine.unit.cell, stateMachine.unit.unitInventory.equiptWeapon.minRange, stateMachine.unit.unitInventory.equiptWeapon.maxRange);
        Unit targetUnit = stateMachine.gameBoard.GetAttackableUnitFromArea(stateMachine.unit.unitGroup, attackableArea);

        bool needsToMove = (targetUnit == null);
        if (needsToMove)
        {
            //expensive path finding, should find different way
            List<Vector2> attackableSpots = new List<Vector2>();
            attackableSpots.AddRange(stateMachine.gameBoard.RangeFloodFill(targetLoc, stateMachine.unit.unitInventory.equiptWeapon.minRange, stateMachine.unit.unitInventory.equiptWeapon.maxRange));
            attackableSpots.Remove(targetLoc);

            List<Vector2> path = new List<Vector2>();
            Vector2[] currentPath;
            if (attackableSpots.Count > 0)
            {
                foreach (Vector2 loc in attackableSpots)
                {
                    if (stateMachine.gameBoard.IsOccupied(loc))
                    {
                        continue;
                    }
                    if (stateMachine.gameBoard.map.GetTileMoveCost(loc) > 100)
                    {
                        continue;
                    }

                    currentPath = stateMachine.gameBoard.DijkstraPathFinding(stateMachine.unit.cell, loc, stateMachine.unit.unitActionEconomy.currentMove);
                    if (currentPath.Length == 0)
                    {
                        continue;
                    }
                    if (path.Count == 0 || currentPath.Length < path.Count)
                    {
                        path.Clear();
                        path.AddRange(currentPath);
                    }
                }
            }

            if (path.Count > 0)
            {
                path.RemoveAt(0);
                MoveLogic(path.ToArray(), stateMachine.unit.unitActionEconomy.currentMove);
            }
            if (isWalking)
            {
                isWalking = false;
                await ToSignal(stateMachine.gameBoard, "SelectedMoved");
            }
        }

        if (needsToMove)
        {
            attackableArea = stateMachine.gameBoard.RangeFloodFill(stateMachine.unit.cell, stateMachine.unit.unitInventory.equiptWeapon.minRange, stateMachine.unit.unitInventory.equiptWeapon.maxRange);
            targetUnit = stateMachine.gameBoard.GetAttackableUnitFromArea(stateMachine.unit.unitGroup, attackableArea);
        }

        if (targetUnit != null)
        {
            stateMachine.gameBoard.combatManager.UnitCombat(stateMachine.unit, targetUnit);
        }

        stateMachine.UnitFinsh();
    }

    /// <summary>
    /// the move logic for the state
    /// </summary>
    /// <param name="path">path that unit will take</param>
    /// <param name="moveLimit">move of unit</param>
    private void MoveLogic(Vector2[] path, float moveLimit)
    {
        float currentMoveCost = 0;
        int validIndex = -1;

        for (int i = 0; i < path.Length; i++)
        {
            if (!stateMachine.gameBoard.CheckCanPass(stateMachine.unit, path[i]))
            {
                break;
            }
            currentMoveCost += stateMachine.gameBoard.map.GetTileMoveCost(path[i]);
            if (currentMoveCost > moveLimit)
            {
                break;
            }
            if (stateMachine.gameBoard.IsOccupied(path[i]))
            {
                continue;
            }

            validIndex = i;
        }

        if (validIndex != -1)
        {
            stateMachine.LogicNeedsAwait();
            isWalking = true;
            stateMachine.gameBoard.SelectUnit(stateMachine.unit.cell);
            stateMachine.gameBoard.DrawAutoPathForAi(stateMachine.unit.cell, path[validIndex]);
            stateMachine.gameBoard.MoveSelectedUnit(path[validIndex]);
        }
    }
}
