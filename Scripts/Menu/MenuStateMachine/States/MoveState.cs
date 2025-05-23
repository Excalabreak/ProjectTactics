using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/23/2025]
 * [State when move is selected]
 */

public partial class MoveState : MenuState
{
    public override void OnCursorAccept(Vector2 cell)
    {
        stateMachine.gameBoard.OnMoveAction(cell);
    }
}
