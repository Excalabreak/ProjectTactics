using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/15/2025]
 * [outlines the grid of map]
 */

public partial class GridOutline : TileMapLayer
{
    [Export] private GameBoard _gameboard;

    public override void _Ready()
    {
        Vector2I coords;
        int sourceID = 0;
        Vector2I atlasCoords = Vector2I.Zero;
        int altTiles = 0;

        for (int i = 0; i < Mathf.RoundToInt(_gameboard.grid.gridSize.Y); i++)
        {
            for (int j = 0; j < Mathf.RoundToInt(_gameboard.grid.gridSize.X); j++)
            {
                coords = new Vector2I(j, i);
                SetCell(coords, sourceID, atlasCoords, altTiles);
            }
        }
    }
}
