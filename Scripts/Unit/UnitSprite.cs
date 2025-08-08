using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/29/2025]
 * [script for the unit and the sprite]
 */

public partial class UnitSprite : Sprite2D
{
    [Export] private Unit _unit;
    private Texture2D _skin;
    private Vector2 _skinOffset = Vector2.Zero;

    public override void _Ready()
    {
        skin = _unit.unitResource.sprite;
        skinOffset = _unit.unitResource.spriteOffset;
    }

    //properties
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
