using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [puts a tile when called for unit locations]
 */

public partial class KnownUnitLocations : TileMapLayer
{
    /// <summary>
    /// puts a tile where there is a known unit
    /// </summary>
    /// <param name="coords">coordinates of units</param>
    public void MarkKnownUnit(Vector2 coords)
    {
        Vector2I mapCoords = new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y));

        SetCell(mapCoords, 0, Vector2I.Zero, 0);
    }

    /// <summary>
    /// reveals map cell
    /// </summary>
    /// <param name="coords">coordinates of tile to show</param>
    public void RemoveMarkedUnit(Vector2 coords)
    {
        EraseCell(new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y)));
    }
}
