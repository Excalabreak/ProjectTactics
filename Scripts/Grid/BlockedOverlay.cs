using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/13/2025]
 * [shows which tiles are blocking vision
 * mostly here for testing ]
 */

public partial class BlockedOverlay : TileMapLayer
{
    /// <summary>
    /// hides the map cell by covering it with a second tile map
    /// </summary>
    /// <param name="coords">coordinates of tile to hide</param>
    public void BlockCell(Vector2 coords)
    {
        Vector2I mapCoords = new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y));

        SetCell(mapCoords, 0, Vector2I.Zero, 0);
    }

    /// <summary>
    /// reveals map cell
    /// </summary>
    /// <param name="coords">coordinates of tile to show</param>
    public void RemoveBlockCell(Vector2 coords)
    {
        EraseCell(new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y)));
    }
}
