using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [05/21/2025]
 * [menu for unit action
 * NOTE: can probably make this and pause menu the same script
 * will do after tutorial]
 */

public partial class ActionMenu : CanvasLayer
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

    private void OnAttackButtonPress()
    {

    }

    private void OnSkillButtonPress()
    {

    }
    private void OnTalkButtonPress()
    {

    }

    private void OnMoveButtonPress()
    {

    }

    private void OnTurnButtonPress()
    {

    }

    private void OnItemButtonPress()
    {

    }

    private void OnTradeButtonPress()
    {

    }

    /// <summary>
    /// going to put this here for now, but will likely just move it to the
    /// decline buttion
    /// </summary>
    private void OnCancelButtonPress()
    {
        // reset unit pos
        _gameBoard.ResetUnit();

        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }

    private void OnWaitButtonPress()
    {
        //set unit to wait

        _gameBoard.ClearSelectedUnit();

        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
