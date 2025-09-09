using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/02/2025]
 * [menu state for item inventory]
 */

public partial class MenuItemState : MenuState
{
    public override void Enter()
    {
        stateMachine.gameBoard.SpawnItemMenu();
    }
}
