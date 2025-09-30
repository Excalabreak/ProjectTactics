using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/20/2025]
 * [resources for square area warnings]
 */

[GlobalClass]
public partial class WarningAreaResource : Resource
{
    [Export] private Vector2 _spriteCoords;

    [Export] private Vector2 _areaTopLeft;
    [Export] private Vector2 _areaBottomRight;

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

    public Vector2 spriteCoords
    {
        get { return _spriteCoords; }
    }

    public Vector2[] area
    {
        get { return GetSquareArea(_areaTopLeft, _areaBottomRight); }
    }
}
