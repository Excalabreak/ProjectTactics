using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/02/2025]
 * [which function to call for the attack state]
 */

public partial class AttackState : MenuState
{
    public override void Enter()
    {
        stateMachine.gameBoard.ShowCurrentAttackRange();
    }

    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.MenuAttackStateAccept(cell);
    }
}
