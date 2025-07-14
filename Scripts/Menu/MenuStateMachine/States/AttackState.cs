using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [which function to call for the menu attack state]
 */

public partial class AttackState : MenuState
{
    public override void Enter()
    {
        stateMachine.gameBoard.ShowCurrentAttackRange(stateMachine.gameBoard.selectedUnit);
    }

    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.MenuAttackStateAccept(cell);
    }

    public override void OnCursorDecline()
    {
        stateMachine.gameBoard.StandardCursorDecline();
    }

    public override void OnCursorMove(Vector2 newCell)
    {
        stateMachine.gameBoard.MenuAttackStateCursorMove(newCell);
    }

    public override void OnHover(Vector2 cell)
    {
        stateMachine.gameBoard.CombatHoverDisplay(cell);
    }

    public override void Exit()
    {
        stateMachine.gameBoard.HideBattleUI();
    }
}
