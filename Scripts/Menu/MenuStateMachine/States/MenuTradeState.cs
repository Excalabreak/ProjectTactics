using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/25/2025]
 * [menu state for trading items]
 */

public partial class MenuTradeState : MenuState
{
    public override void Enter()
    {
        stateMachine.gameBoard.ShowTradeCells(stateMachine.gameBoard.selectedUnit.cell);
    }

    public override void OnCursorAccept(Vector2 cell)
    {
        //checks if tradeable unit is on cell
        //brings up trade menu
    }

    public override void OnCursorDecline()
    {
        stateMachine.gameBoard.StandardCursorDecline();
    }

    public override void OnCursorMove(Vector2 newCell)
    {
        //show items and trade options
    }

    public override void OnHover(Vector2 cell)
    {
        //shows cell items
    }

    public override void Exit()
    {
        //make sure trade menus are hidden
    }
}
