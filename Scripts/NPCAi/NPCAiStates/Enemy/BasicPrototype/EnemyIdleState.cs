using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/02/2025]
 * [idle state for NPCs]
 */

public partial class EnemyIdleState : NPCAiState
{
    /// <summary>
    /// checks if a unit is in the enemy
    /// range, then attacks
    /// </summary>
    public override void TurnLogic()
    {
        List<Vector2> checkTiles = new List<Vector2>();
        checkTiles.AddRange(stateMachine.gameBoard.GetWalkableCells(stateMachine.unit));
        checkTiles.AddRange(stateMachine.gameBoard.GetAttackableCells(stateMachine.unit));

        if (stateMachine.gameBoard.CheckAreaForAttackableGroup(stateMachine.unit.unitGroup, checkTiles.ToArray()))
        {
            stateMachine.TransitionTo("EnemyAttackState");
            stateMachine.DoTurn();
        }
    }
}
