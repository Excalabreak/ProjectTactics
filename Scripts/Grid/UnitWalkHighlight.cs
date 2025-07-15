using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [Lovato, Nathan]
 * Last Updated: [05/20/2025]
 * [script to handle highlighting walkable tiles
 * also handles attackable tiles, but im too lazy to rename]
 */

public partial class UnitWalkHighlight : TileMapLayer
{
    public void DrawWalkHighlights(Vector2[] cells)
    {
        //Clear();

        foreach (Vector2 cell in cells)
        {
            SetCell(new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y)), 0, Vector2I.Zero, 0);
        }
    }

    public void DrawAttackHighlights(Vector2[] cells)
    {
        foreach (Vector2 cell in cells)
        {
            SetCell(new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y)), 1, Vector2I.Zero, 0);
        }
    }
}
