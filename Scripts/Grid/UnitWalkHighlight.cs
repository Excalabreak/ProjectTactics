using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [09/15/2025]
 * [script to handle highlighting walkable tiles
 * also handles attackable tiles, but im too lazy to rename]
 */

public partial class UnitWalkHighlight : TileMapLayer
{
    /// <summary>
    /// adds walk highlights to cell
    /// </summary>
    /// <param name="cells">coords to highlight</param>
    public void DrawWalkHighlights(Vector2[] cells)
    {
        Vector2I coords;
        int sourceID = 0;
        Vector2I atlasCoords = Vector2I.Zero;
        int altTiles = 0;

        foreach (Vector2 cell in cells)
        {
            coords = new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y));
            SetCell(coords, sourceID, atlasCoords, altTiles);
        }
    }

    /// <summary>
    /// adds attack highlights to cell
    /// </summary>
    /// <param name="cells">coords to highlight</param>
    public void DrawAttackHighlights(Vector2[] cells)
    {
        Vector2I coords;
        int sourceID = 1;
        Vector2I atlasCoords = Vector2I.Zero;
        int altTiles = 0;
        foreach (Vector2 cell in cells)
        {
            coords = new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y));
            SetCell(coords, sourceID, atlasCoords, altTiles);
        }
    }
}
