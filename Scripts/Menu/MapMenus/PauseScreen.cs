using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: YT:Heal Moon
 * Last Updated: [07/09/2025]
 * [menu for unit action]
 */

public partial class PauseScreen : BaseMenu
{
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
}
