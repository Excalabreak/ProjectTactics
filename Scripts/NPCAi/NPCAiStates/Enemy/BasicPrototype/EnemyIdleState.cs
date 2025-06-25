using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/25/2025]
 * [idle state for NPCs]
 */

public partial class EnemyIdleState : NPCAiState
{
    /// <summary>
    /// checks if a unit is in the enemy
    /// range, then attacks
    /// </summary>
    /// <returns></returns>
    public override NPCAiState CheckTrigger()
    {
        List<Vector2> checkTiles = new List<Vector2>();
        checkTiles.AddRange(stateMachine.gameBoard.GetWalkableCells(stateMachine.unit));
        checkTiles.AddRange(stateMachine.gameBoard.GetAttackableCells(stateMachine.unit));

        if (stateMachine.gameBoard.CheckAreaForAttackableGroup(stateMachine.unit.unitGroup, checkTiles.ToArray()))
        {
            return _nextPossibleStates[0];
        }
        return this;
    }
}
