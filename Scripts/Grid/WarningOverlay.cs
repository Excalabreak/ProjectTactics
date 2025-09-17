using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/17/2025]
 * [handles showing and removing warnings]
 */

public partial class WarningOverlay : Node2D
{
    [Export] private TileMapLayer _spriteLayer;
    [Export] private TileMapLayer _areaLayer;

    private Dictionary<Vector2, Vector2[]> _currentAreas = new Dictionary<Vector2, Vector2[]>();

    /// <summary>
    /// adds the warning area and sprite to the map
    /// adds them to dictionary
    /// </summary>
    /// <param name="warningArea">area of cells to highlight</param>
    /// <param name="spriteLocation">where the ! sprite goes</param>
    public void AddWarningArea(Vector2[] warningArea, Vector2 spriteLocation)
    {
        Vector2I coords;
        int sourceID = 0;
        Vector2I atlasCoords = Vector2I.Zero;
        int altTiles = 0;

        if (!HasWarningAt(spriteLocation))
        {
            coords = new Vector2I(Mathf.RoundToInt(spriteLocation.X), Mathf.RoundToInt(spriteLocation.Y));
            _spriteLayer.SetCell(coords, sourceID, atlasCoords, altTiles);
        }
        else
        {
            bool hasNewSpriteLocation = false;
            foreach (Vector2 cell in warningArea)
            {
                if (HasWarningAt(cell))
                {
                    continue;
                }

                coords = new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y));
                _spriteLayer.SetCell(coords, sourceID, atlasCoords, altTiles);
                hasNewSpriteLocation = true;
                break;
            }
            if (!hasNewSpriteLocation)
            {
                GD.Print("no viable location for warning sprites");
                return;
            }
        }

        foreach (Vector2 cell in warningArea)
        {
            coords = new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y));
            _areaLayer.SetCell(coords, sourceID, atlasCoords, altTiles);
        }

        _currentAreas.Add(spriteLocation, warningArea);
    }

    /// <summary>
    /// removes area from game board
    /// </summary>
    /// <param name="areaSprite">where the ! sprite is</param>
    public void RemoveWarningArea(Vector2 areaSprite)
    {
        if (!HasWarningAt(areaSprite))
        {
            GD.Print("not valid warning area sprite location");
            return;
        }

        Vector2I coords;

        coords = new Vector2I(Mathf.RoundToInt(areaSprite.X), Mathf.RoundToInt(areaSprite.Y));
        _spriteLayer.EraseCell(coords);

        foreach (Vector2 cell in _currentAreas[areaSprite])
        {
            coords = new Vector2I(Mathf.RoundToInt(cell.X), Mathf.RoundToInt(cell.Y));
            _areaLayer.EraseCell(coords);
        }
    }

    /// <summary>
    /// returns if there is a warning sprite at coords
    /// </summary>
    /// <param name="coord">where is the warning sprite</param>
    /// <returns>true if yes</returns>
    public bool HasWarningAt(Vector2 coord)
    {
        return _currentAreas.ContainsKey(coord);
    }
}
