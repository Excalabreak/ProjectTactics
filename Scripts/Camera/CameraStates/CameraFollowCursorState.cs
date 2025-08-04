using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/03/2025]
 * [state for camera following the grid cursor]
 */

public partial class CameraFollowCursorState : CameraState
{
    /// <summary>
    /// sets the camera settings for camera follow
    /// </summary>
    public override void Enter()
    {
        if (!stateMachine.ignoredReady)
        {
            stateMachine.HasIgnoredReady();
        }
        else
        {
            stateMachine.camera.AttachToCursor();
        }

        stateMachine.camera.DragHorizontalEnabled = true;
        stateMachine.camera.DragVerticalEnabled = true;
    }
}
