using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/09/2025]
 * [State when move is selected]
 */

public partial class MoveState : MenuState
{
    public override void Enter()
    {
        //probably a terrable way of doing this...
        //oh well
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
}
