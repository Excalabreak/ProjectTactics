using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [12/25/2025]
 * [pannel to display info for development]
 */

public partial class InfoPanel : CanvasLayer
{
    [Export] private GameBoard _gameBoard;
    [Export] private Label _currentSelected;

    public override void _Process(double delta)
    {
        if (_gameBoard.selectedUnit != null)
        {
            _currentSelected.Text = "Current Selected: " + _gameBoard.selectedUnit.Name;
        }
        else
        {
            _currentSelected.Text = "Current Selected: ";
        }
    }
}
