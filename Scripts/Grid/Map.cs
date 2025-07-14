using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Radical Oyster/Heal Moon]
 * Last Updated: [06/30/2025]
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
        float[,] output = new float[Mathf.RoundToInt(grid.gridSize.Y), Mathf.RoundToInt(grid.gridSize.X)];
        for (int i = 0; i < grid.gridSize.Y; i++)
        {
            for (int j = 0; j < grid.gridSize.X; j++)
            {
                output[i, j] = (float)GetCellTileData(new Vector2I(j,i)).GetCustomData("MoveCost");
            }
        }
        return output;
    }

    /// <summary>
    /// gets the move cost of a list of coords
    /// </summary>
    /// <param name="path">array of coordinates</param>
    /// <returns>move cost of path</returns>
    public float GetPathMoveCost(Vector2I[] path)
    {
        float output = 0;
        foreach (Vector2I tile in path)
        {
            output += (float)GetCellTileData(tile).GetCustomData("MoveCost");
        }
        return output;
    }

    /// <summary>
    /// gets the move cost of a list of coords
    /// </summary>
    /// <param name="path">array of coordinates</param>
    /// <returns>move cost of path</returns>
    public float GetPathMoveCost(Vector2[] path)
    {
        if (path == null)
        {
            return 0;
        }

        float output = 0;

        foreach (Vector2 tile in path)
        {
            GD.Print(tile);
            Vector2I iTile = new Vector2I(Mathf.RoundToInt(tile.X), Mathf.RoundToInt(tile.X));
            output += (float)GetCellTileData(iTile).GetCustomData("MoveCost");
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

    /// <summary>
    /// gets the move cost of a singular tile
    /// </summary>
    /// <param name="tile">coordinates of tile</param>
    /// <returns>move cost of coordinates</returns>
    public float GetTileMoveCost(Vector2I tile)
    {
        return (float)GetCellTileData(tile).GetCustomData("MoveCost");
    }

    /// <summary>
    /// gets the move cost of a singular tile
    /// </summary>
    /// <param name="tile">coordinates of tile</param>
    /// <returns>move cost of coordinates</returns>
    public float GetTileMoveCost(Vector2 tile)
    {
        return (float)GetCellTileData(new Vector2I(Mathf.RoundToInt(tile.X), Mathf.RoundToInt(tile.Y)))
            .GetCustomData("MoveCost");
    }
}
