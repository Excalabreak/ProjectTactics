using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Heal Moon]
 * Last Updated: [05/01/2025]
 * [Hold data about terrain]
 */

public partial class TerrainData : TileSet
{
    [Export] private Dictionary<int, float> _moveCost;

    public Dictionary<int, float> moveCost
    {
        get { return _moveCost; }
    }
}
