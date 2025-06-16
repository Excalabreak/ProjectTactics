using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/09/2025]
 * [State when no unit is selected]
 */

public partial class UnSelectedState : MenuState
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
}
