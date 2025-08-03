using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/03/2025]
 * [script for the camera]
 */

public partial class GameBoardCamera : Camera2D
{
    [Export] private CameraStateMachine _cameraStateMachine;
    [Export] private RemoteTransform2D _remoteTransform;

    private bool _dragCamera = false;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;

            if (_dragCamera)
            {
                Position -= mouseMotion.Relative * Zoom;
                GetViewport().SetInputAsHandled();
            }
        }

        if (@event.IsActionPressed("DragCamera"))
        {
            _dragCamera = true;
        }
        else if (@event.IsActionReleased("DragCamera"))
        {
            _dragCamera = false;
        }
    }

    //properties

    public CameraStateMachine cameraStateMachine
    {
        get { return _cameraStateMachine; }
    }

    public RemoteTransform2D remoteTransform
    {
        get { return _remoteTransform; }
    }
}
