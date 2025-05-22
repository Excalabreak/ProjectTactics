using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [05/22/2025]
 * [menu for unit action]
 */

public partial class PauseScreen : CanvasLayer
{
    private GridCursor _gridCursor;
    [Export]private Button _cursorDefaultStartButton;

    public override void _Ready()
    {
        GameBoard gameBoard = GetParent() as GameBoard;

        if (gameBoard == null)
        {
            GD.PrintErr("MENU ISN'T CHILD OF GAMEBOARD, SHIT HIT THE FAN");
        }

        _gridCursor = gameBoard.gridCursor;
        _cursorDefaultStartButton.GrabFocus();

        //disables cursor
        _gridCursor.Hide();
        _gridCursor.ProcessMode = ProcessModeEnum.Disabled;
    }

    private void OnUnitsPressed()
    {

    }

    private void OnOptionsPressed()
    {

    }

    private void OnClosePressed()
    {

        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }

    private void OnEndTurnPressed()
    {

    }
}
