using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [09/17/2025]
 * [menu for unit action]
 */

public partial class PauseScreen : BaseMenu
{
    [Export] private Button _removeWarningButton;

    private Vector2 _menuOpenAt;

    private void OnUnitsPressed()
    {

    }

    private void OnOptionsPressed()
    {

    }

    private void OnRemoveWarningPress()
    {
        HideMenu();
        _gameBoard.warningOverlay.RemoveWarningArea(_menuOpenAt);
    }

    public void ShowRemoveWarning(bool show, Vector2 cell)
    {
        _removeWarningButton.Visible = show;
        if (show)
        {
            _menuOpenAt = cell;
        }
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
}
