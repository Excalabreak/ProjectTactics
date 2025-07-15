using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/02/2025]
 * [outlines the grid of map]
 */

public partial class GridOutline : TileMapLayer
{
    [Export] private GameBoard _gameboard;

    public override void _Ready()
    {
        for (int i = 0; i < Mathf.RoundToInt(_gameboard.grid.gridSize.Y); i++)
        {
            for (int j = 0; j < Mathf.RoundToInt(_gameboard.grid.gridSize.X); j++)
            {
                SetCell(new Vector2I(j, i), 0, Vector2I.Zero, 0);
            }
        }
    }
}
