using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/17/2025]
 * [UI element for terrain]
 */

public partial class UITerrain : Control
{
    [Export] private Label _nameLabel;
    [Export] private Label _moveLabel;
    [Export] private Label _visionLabel;

    public void ShowTerrainPanel(string name, float move, float vision)
    {
        _nameLabel.Text = name;
        _moveLabel.Text = "Move Cost: " + move;
        _visionLabel.Text = "Vision Cost: " + vision;

        this.Visible = true;
    }

    /// <summary>
    /// hides the stats pannel
    /// </summary>
    public void HideTerrainPanel()
    {
        this.Visible = false;
    }
}
