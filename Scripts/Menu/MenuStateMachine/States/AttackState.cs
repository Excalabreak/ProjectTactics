using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/30/2025]
 * [which function to call for the attack state]
 */

public partial class AttackState : MenuState
{
    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.OnAttackAction(cell);
    }
}
