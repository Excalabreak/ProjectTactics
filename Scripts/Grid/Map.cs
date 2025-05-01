using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Radical Oyster/Heal Moon]
 * Last Updated: [05/01/2025]
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
        float[,] output = new float[(int)Mathf.Round(grid.gridCell.Y), (int)Mathf.Round(grid.gridCell.X)];
        for (int i = 0; i < grid.gridCell.Y; i++)
        {
            for (int j = 0; j < grid.gridCell.X; j++)
            {
                output[i, j] = (float)GetCellTileData(new Vector2I(j,i)).GetCustomData("MoveCost");
            }
        }
        return output;
    }
}
