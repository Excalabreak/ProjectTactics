using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/01/2025]
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

        List<Vector2> path = new List<Vector2>();
        path.AddRange(stateMachine.gameBoard.DijkstraPathFinding(stateMachine.unit.cell, targetLoc));
        path.RemoveAt(0);

        for (int i = 0; i < stateMachine.unit.attackRange; i++)
        {
            if (path.Count <= 0)
            {
                break;
            }
            path.RemoveAt(path.Count - 1);
        }

        if (path.Count > 0)
        {
            
            MoveLogic(path.ToArray(), stateMachine.unit.unitActionEconomy.currentMove);
        }
        if (isWalking)
        {
            isWalking = false;
            await ToSignal(stateMachine.gameBoard, "SelectedMoved");
        }

        Vector2[] attackableArea = stateMachine.gameBoard.FloodFill(stateMachine.unit.cell, stateMachine.unit.attackRange);
        Unit targetUnit = stateMachine.gameBoard.GetAttackableUnitFromArea(stateMachine.unit.unitGroup, attackableArea);

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
