using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [State when no unit is selected]
 */

public partial class MenuUnSelectedState : MenuState
{
    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.MenuUnSelectedStateAccept(cell);
    }

    public override void OnCursorMove(Vector2 newCell)
    {
        stateMachine.gameBoard.MenuUnSelectedStateCursorMove(newCell);
    }

    public override void OnCursorDecline()
    {
        stateMachine.gameBoard.StandardCursorDecline();
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
