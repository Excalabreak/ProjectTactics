using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/14/2025]
 * [Unit Main Script
 * NOTE: all of this is one script for now,
 * but will likely need to be split up into different
 * scripts. comments of node type will likely be where they split]
 */

[Tool]
public partial class Unit : Node2D
{
    //maybe somewhere else, but will likely leave in this script
    private Vector2 _cell = Vector2.Zero;
    private bool _isSelected = false;

    [Export] private AnimationPlayer _animPlayer; 

    //map
    [Export] private GridResource _grid;

    //path2d
    [Signal] public delegate void WalkFinishedEventHandler();
    [Export] private Path2D _path;
    [Export] private PathFollow2D _pathFollow;
    private bool _isWalking = false;

    //sprite2d
    [Export] private Sprite2D _sprite;
    [Export] private Texture2D _skin;
    [Export] private Vector2 _skinOffset = Vector2.Zero;

    //unit stats
    [Export] private int _moveRange = 6;

    //settings
    [Export] private float _moveSpeed = 600f;

    

    //--- PROPERTIES ---
    public Vector2 cell
    {
        set
        {
            if (_grid != null)
            {
                _cell = _grid.Clamp(value);
            }
        }
        get { return _cell; }
    }

    public bool isSelected
    {
        set
        {
            _isSelected = value;
            if (_animPlayer != null)
            {
                if (_isSelected)
                {
                    _animPlayer.Play("UnitTestSelected");
                }
                else
                {
                    _animPlayer.Play("UnitTestIdle");
                }
            }
            else
            {
                GD.Print("no AnimationPlayer Node for is selected to be set on");
            }
        }
        get { return _isSelected; }
    }

    public bool isWalking 
    {
        set
        {
            _isWalking = value;
            SetProcess(_isWalking);
        }
        get { return _isWalking; }
    }

    public Texture2D skin
    {
        set 
        { 
            _skin = value;
            if (_sprite != null)
            {
                _sprite.Texture = value;
            }
            else
            {
                GD.Print("no Sprite2D Node for texture to be set on");
            }
        }
        get { return _skin; }
    }

    public Vector2 skinOffset
    {
        set
        {
            _skinOffset = value;
            if (_sprite != null)
            {
                _sprite.Position = value;
            }
            else
            {
                GD.Print("not Sprite2D Node for offset to be set on");
            }
        }
        get { return _skinOffset; }
    }
}
