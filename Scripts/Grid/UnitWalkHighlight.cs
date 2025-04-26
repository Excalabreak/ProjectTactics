using Godot;
using System;

public partial class UnitWalkHighlight : TileMapLayer
{
    public void DrawHighlights(Vector2[] cells)
    {
        Clear();

        foreach (Vector2 cell in cells)
        {
            SetCell(new Vector2I((int)Mathf.Round(cell.X), (int)Mathf.Round(cell.Y)), 0, Vector2I.Zero, 0);
        }
    }
}
