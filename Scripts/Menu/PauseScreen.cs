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
    private GameBoard _gameBoard;
    private GridCursor _gridCursor;

    [Export]private Button _cursorDefaultStartButton;

    public override void _Ready()
    {
        _gameBoard = GetParent() as GameBoard;

        if (_gameBoard == null)
        {
            GD.PrintErr("MENU ISN'T CHILD OF GAMEBOARD, SHIT HIT THE FAN");
        }

        _gridCursor = _gameBoard.gridCursor;
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

    /// <summary>
    /// closes pause menu
    /// </summary>
    public void OnClosePressed()
    {
        HideMenu();
    }

    private void OnEndTurnPressed()
    {
        HideMenu();
        _gameBoard.EndTurn();
    }

    private void HideMenu()
    {
        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
