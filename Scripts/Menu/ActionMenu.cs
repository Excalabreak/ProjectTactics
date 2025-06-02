using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [05/30/2025]
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
        
        //add a function to show and hide action menu buttons
    }

    private void OnAttackButtonPress()
    {
        HideMenu();
        _gameBoard.menuStateMachine.TransitionTo("AttackState");
    }

    private void OnSkillButtonPress()
    {

    }
    private void OnTalkButtonPress()
    {

    }

    private void OnMoveButtonPress()
    {
        HideMenu();
        _gameBoard.menuStateMachine.TransitionTo("MoveState");
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
    public void OnCancelButtonPress()
    {
        _gameBoard.ResetMenu();

        HideMenu();
    }

    private void HideMenu()
    {
        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
