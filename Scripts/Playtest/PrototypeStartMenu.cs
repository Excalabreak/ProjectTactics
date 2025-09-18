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
        Vector2 startArea;
        Vector2 endArea;
        Vector2 warningSprite;
        //depending on which option is selected, show the scouting warnings
        switch (_scoutingOptions.Selected)
        {
            case 0:
                startArea = new Vector2(1,10);
                endArea = new Vector2(5,14);
                warningSprite = new Vector2(3, 12);
                _warningOverlay.AddWarningArea(GetSquareArea(startArea, endArea), warningSprite);
                break;
            case 1:
                startArea = new Vector2(6, 10);
                endArea = new Vector2(12, 14);
                warningSprite = new Vector2(8, 12);
                _warningOverlay.AddWarningArea(GetSquareArea(startArea, endArea), warningSprite);
                break;
            case 2:
                startArea = new Vector2(7,15);
                endArea = new Vector2(12,18);
                warningSprite = new Vector2(9, 17);
                _warningOverlay.AddWarningArea(GetSquareArea(startArea, endArea), warningSprite);
                break;
            default:
                break;
        }

        _gridCursor.ProcessMode = ProcessModeEnum.Inherit;
        _gridCursor.ResetCursor();
        _gridCursor.Show();
        QueueFree();
    }

    /// <summary>
    /// a temp method to get an area for the scouting area
    /// </summary>
    /// <param name="start">top left of area</param>
    /// <param name="end">bottom right of area</param>
    /// <returns>array of vector2</returns>
    private Vector2[] GetSquareArea(Vector2 start, Vector2 end)
    {
        List<Vector2> output = new List<Vector2>();

        for (int i = Mathf.RoundToInt(start.X); i <= Mathf.RoundToInt(end.X); i++)
        {
            for (int j = Mathf.RoundToInt(start.Y); j <= Mathf.RoundToInt(end.Y); j++)
            {
                output.Add(new Vector2(i, j));
            }
        }

        return output.ToArray();
    }
}
