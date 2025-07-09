using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/09/2025]
 * [base menu for all menu on maps]
 */

public partial class BaseMenu : CanvasLayer
{
    protected GameBoard _gameBoard;
    protected GridCursor _gridCursor;

    [Export] protected Button _cursorDefaultStartButton;

    /// <summary>
    /// sets up needed components
    /// </summary>
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

   /// <summary>
   /// hides the menu by queue free
   /// </summary>
    protected void HideMenu()
    {
        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
