using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/02/2025]
 * [script for the camera]
 */

public partial class GameBoardCamera : Camera2D
{
    [Export] private CameraStateMachine _cameraStateMachine;

    public CameraStateMachine cameraStateMachine
    {
        get { return _cameraStateMachine; }
    }
}
