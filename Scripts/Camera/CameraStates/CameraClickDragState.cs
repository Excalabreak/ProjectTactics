using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/03/2025]
 * [state for click and drag]
 */

public partial class CameraClickDragState : CameraState
{
    /// <summary>
    /// sets the camera settings for camera follow
    /// </
    public override void Enter()
    {
        stateMachine.camera.remoteTransform.UpdatePosition = false;
        stateMachine.camera.DragHorizontalEnabled = false;
        stateMachine.camera.DragVerticalEnabled = false;
    }
}
