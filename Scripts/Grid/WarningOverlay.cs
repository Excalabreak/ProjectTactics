using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/11/2025]
 * [handles showing and removing warnings]
 */

public partial class WarningOverlay : TileMapLayer
{
    /// <summary>
    /// shows a warning cell at cell
    /// </summary>
    /// <param name="coords">coordinates of tile to hide</param>
    public void AddWarningCell(Vector2 coords)
    {
        Vector2I mapCoords = new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y));

        SetCell(mapCoords, 0, Vector2I.Zero, 0);
    }

    /// <summary>
    /// removes warning cell
    /// </summary>
    /// <param name="coords">coordinates of tile to show</param>
    public void RemoveWarningCell(Vector2 coords)
    {
        EraseCell(new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y)));
    }
}
