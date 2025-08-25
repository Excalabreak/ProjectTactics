using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/09/2025]
 * [a menu for turning a unit]
 */

public partial class TurnMenu : BaseMenu
{
    private void TurnUnit(DirectionEnum direction)
    {
        if (_gameBoard.selectedUnit.unitDirection.currentFacing == direction)
        {
            //might need to expand if menus are made ontop of eachother
            OnCancelPressed();
            _gameBoard.menuStateMachine.TransitionTo("MenuUnSelectedState");
            return;
        }

        _gameBoard.selectedUnit.unitDirection.currentFacing = direction;
        _gameBoard.menuStateMachine.TransitionTo("MenuUnSelectedState");
        OnCancelPressed();
    }

    private void OnUpPressed()
    {
        TurnUnit(DirectionEnum.UP);
    }

    private void OnDownPressed()
    {
        TurnUnit(DirectionEnum.DOWN);
    }

    private void OnLeftPressed()
    {
        TurnUnit(DirectionEnum.LEFT);
    }

    private void OnRightPressed()
    {
        TurnUnit(DirectionEnum.RIGHT);
    }

    public void OnCancelPressed()
    {
        _gameBoard.ResetMenu();
        HideMenu();
    }
}
