using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/27/2025]
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
        GD.Print(stateMachine.gameBoard.ClosestUnitPosition(UnitGroupEnum.PLAYER, stateMachine.unit.cell));
    }
}
