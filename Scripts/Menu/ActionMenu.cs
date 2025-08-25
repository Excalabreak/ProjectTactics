using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [07/09/2025]
 * [menu for unit action
 * NOTE: can probably make this and pause menu the same script
 * will do after tutorial]
 */

public partial class ActionMenu : BaseMenu
{
    [Export] private Button[] _actionButtons;

    public override void _Ready()
    {
        base._Ready();
        ActionVisibility(_gameBoard.selectedUnit.unitActionEconomy.HasActions());
    }

    private void OnAttackButtonPress()
    {
        HideMenu();
        _gameBoard.menuStateMachine.TransitionTo("MenuAttackState");
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
        _gameBoard.menuStateMachine.TransitionTo("MenuMoveState");
    }

    private void OnTurnButtonPress()
    {
        HideMenu();
        _gameBoard.SpawnTurnMenu();
    }

    private void OnItemButtonPress()
    {

    }

    private void OnTradeButtonPress()
    {

    }

    /// <summary>
    /// shows or hides actionsbased on bool
    /// </summary>
    /// <param name="visible">true if unit has actions</param>
    private void ActionVisibility(bool visible)
    {
        foreach (Button button in _actionButtons)
        {
            button.Visible = visible;
        }
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
}
