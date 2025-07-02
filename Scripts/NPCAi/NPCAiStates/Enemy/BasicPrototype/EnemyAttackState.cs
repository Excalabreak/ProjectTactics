using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/02/2025]
 * [attack state for enemy ai
 * NOTE: very basic for now]
 */

public partial class EnemyAttackState : NPCAiState
{
    /// <summary>
    /// finds the closest unit and goes for
    /// attacks
    /// </summary>
    public override void TurnLogic()
    {
        Vector2 target = stateMachine.gameBoard.ClosestUnitPosition(UnitGroupEnum.PLAYER, stateMachine.unit.cell);

        List<Vector2> path = new List<Vector2>();
        path.AddRange(stateMachine.gameBoard.DijkstraPathFinding(stateMachine.unit.cell, target));
        path.RemoveAt(0);
        path.RemoveAt(path.Count - 1);

        if (path.Count > 0)
        {
            MoveLogic(path.ToArray(), stateMachine.unit.unitStats.currentMove);
        }

    }

    private void MoveLogic(Vector2[] path, float moveLimit)
    {
        float currentMoveCost = 0;
        int validIndex = -1;

        for (int i = 0; i < path.Length; i++)
        {
            if (stateMachine.gameBoard.IsOccupied(path[i]))
            {
                break;
            }
            currentMoveCost += stateMachine.gameBoard.map.GetTileMoveCost(path[i]);
            if (currentMoveCost > moveLimit)
            {
                break;
            }

            validIndex = i;
        }

        if (validIndex != -1)
        {
            stateMachine.gameBoard.DrawAutoPathForAi(stateMachine.unit.cell, path[validIndex]);
            stateMachine.gameBoard.MoveSelectedUnit(path[validIndex]);
        }
    }
}
