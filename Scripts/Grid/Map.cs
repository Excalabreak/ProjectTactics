using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Radical Oyster/Heal Moon]
 * Last Updated: [05/20/2025]
 * [gives data about tiles in the map]
 */

public partial class Map : TileMapLayer
{
    /// <summary>
    /// gets the movement cost of all tiles in map
    /// </summary>
    /// <param name="grid">grid of the map</param>
    /// <returns>movement cost of all tiles in map</returns>
    public float[,] GetMovementCosts(GridResource grid)
    {
        float[,] output = new float[Mathf.RoundToInt(grid.gridCell.Y), Mathf.RoundToInt(grid.gridCell.X)];
        for (int i = 0; i < grid.gridCell.Y; i++)
        {
            for (int j = 0; j < grid.gridCell.X; j++)
            {
                output[i, j] = (float)GetCellTileData(new Vector2I(j,i)).GetCustomData("MoveCost");
            }
        }
        return output;
    }

    /// <summary>
    /// gets the vision cost of a singular tile
    /// </summary>
    /// <param name="tile">coordinates of tile</param>
    /// <returns>vision cost of coordinates</returns>
    public float GetTileVisionCost(Vector2I tile)
    {
        return (float)GetCellTileData(tile).GetCustomData("VisionCost");
    }

    public float GetTileMoveCost(Vector2I tile)
    {
        return (float)GetCellTileData(tile).GetCustomData("MoveCost");
    }
}
