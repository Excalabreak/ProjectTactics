using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/18/2025]
 * [start menu for playtest]
 */

public partial class PrototypeStartMenu : CanvasLayer
{
    [Export] private GridCursor _gridCursor;
    [Export] private Button _cursorDefaultStartButton;

    /// <summary>
    /// sets up needed components
    /// </summary>
    public override void _Ready()
    {
        this.Visible = true;
        _cursorDefaultStartButton.GrabFocus();

        //disables cursor
        _gridCursor.Hide();
        _gridCursor.ProcessMode = ProcessModeEnum.Disabled;
    }

    /// <summary>
    /// hides the menu by queue free
    /// </summary>
    public void HideMenu()
    {
        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
