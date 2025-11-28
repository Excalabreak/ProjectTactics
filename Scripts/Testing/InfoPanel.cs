using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [11/27/2025]
 * [pannel to display info for development]
 */

public partial class InfoPanel : CanvasLayer
{
    [Export] private GameBoard _gameBoard;
    [Export] private Label _currentSelected;

    public override void _Process(double delta)
    {
        _currentSelected.Text = "Current Selected: " + _gameBoard.selectedUnit.Name;
    }
}
