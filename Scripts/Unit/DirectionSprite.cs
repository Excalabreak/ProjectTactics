using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [05/05/2025]
 * [mostly a test script to show the current facing]
 */

public partial class DirectionSprite : Sprite2D
{
    [Export] private UnitDirection _unitDirection;

    /// <summary>
    /// subs to event
    /// </summary>
    public override void _Ready()
    {
        _unitDirection.UpdateCurrentFacing += UpdateDirectionSprite;
    }

    /// <summary>
    /// unsub event
    /// </summary>
    public override void _ExitTree()
    {
        _unitDirection.UpdateCurrentFacing -= UpdateDirectionSprite;
    }

    /// <summary>
    /// changes rotation based on direction
    /// </summary>
    /// <param name="direction">DirectionEnum</param>
    private void UpdateDirectionSprite(DirectionEnum direction)
    {
        RotationDegrees = DirectionManager.Instance.GetRotationDirection(direction);
    }
}
