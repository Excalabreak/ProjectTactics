using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/17/2025]
 * [start menu for playtest]
 */

public partial class PrototypeStartMenu : CanvasLayer
{
    [Export] private WarningOverlay _warningOverlay;
    [Export] private GridCursor _gridCursor;
    [Export] private Button _cursorDefaultStartButton;
    [Export] private OptionButton _scoutingOptions;

    //TEMP. i hate this, but need prototype
    [Export] private WarningAreaResource[] scoutOne;
    [Export] private WarningAreaResource[] scoutTwo;
    [Export] private WarningAreaResource[] scoutThree;
    
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
        //depending on which option is selected, show the scouting warnings
        switch (_scoutingOptions.Selected)
        {
            case 0:
                foreach (WarningAreaResource areaResource in scoutOne)
                {
                    _warningOverlay.AddWarningArea(areaResource.area, areaResource.spriteCoords);
                }
                break;
            case 1:
                foreach (WarningAreaResource areaResource in scoutTwo)
                {
                    _warningOverlay.AddWarningArea(areaResource.area, areaResource.spriteCoords);
                }
                break;
            case 2:
                foreach (WarningAreaResource areaResource in scoutThree)
                {
                    _warningOverlay.AddWarningArea(areaResource.area, areaResource.spriteCoords);
                }
                break;
            default:
                break;
        }

        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }
}
