using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/18/2025]
 * [script for the unit and the sprite]
 */

public partial class UnitSprite : Sprite2D
{
    [Export] private Texture2D _skin;
    [Export] private Vector2 _skinOffset = Vector2.Zero;

    public Texture2D skin
    {
        set
        {
            _skin = value;
            Texture = value;
        }
        get { return _skin; }
    }

    public Vector2 skinOffset
    {
        set
        {
            _skinOffset = value;
            Position = value;
        }
        get { return _skinOffset; }
    }
}
