using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/13/2025]
 * [Fog of war that obscures enemies when player can't see them
 * NOTE, FOG OF WAR SPRITE SHEET AND REGULAR MAP SPRITE SHEET HAS TO MATCH]
 */

public partial class FogOfWar : TileMapLayer
{
    [Export] private GameBoard _gameboard;
    [Export] private Map _map;

    /// <summary>
    /// hides the whole map
    /// </summary>
    public void HideWholeMap()
    {
        for (int i = 0; i < Mathf.RoundToInt(_gameboard.grid.gridSize.Y); i++)
        {
            for (int j = 0; j < Mathf.RoundToInt(_gameboard.grid.gridSize.X); j++)
            {
                HideMapCell(new Vector2(j, i));
            }
        }
    }

    /// <summary>
    /// hides the map cell by covering it with a second tile map
    /// </summary>
    /// <param name="coords">coordinates of tile to hide</param>
    public void HideMapCell(Vector2 coords)
    {
        Vector2I mapCoords = new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y));
        
        SetCell(mapCoords, 0, _map.GetCellAtlasCoords(mapCoords), 0);
    }

    /// <summary>
    /// reveals map cell
    /// </summary>
    /// <param name="coords">coordinates of tile to show</param>
    public void RevealMapCell(Vector2 coords)
    {
        EraseCell(new Vector2I(Mathf.RoundToInt(coords.X), Mathf.RoundToInt(coords.Y)));
    }
}
