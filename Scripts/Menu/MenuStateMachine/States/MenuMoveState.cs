using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [State when move is selected]
 */

public partial class MenuMoveState : MenuState
{
    public override void Enter()
    {
        stateMachine.gameBoard.HoverDisplay(stateMachine.gameBoard.selectedUnit.cell);
        stateMachine.gameBoard.ResetMovePath();
    }

    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.MenuMoveStateAccept(cell);
    }

    public override void OnCursorMove(Vector2 newCell)
    {
        stateMachine.gameBoard.MenuMoveStateCursorMove(newCell);
    }

    public override void OnCursorDecline()
    {
        stateMachine.gameBoard.MoveStateCursorDecline();
    }

    public override void OnHover(Vector2 cell)
    {
        stateMachine.gameBoard.BaseHoverDisplay(cell);
    }

    public override void Exit()
    {
        stateMachine.gameBoard.HideStatsUI();
    }
}
