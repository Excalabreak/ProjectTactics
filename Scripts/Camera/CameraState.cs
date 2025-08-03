using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/02/2025]
 * [state for camera]
 */

public partial class CameraState : Node
{
    public CameraStateMachine stateMachine;

    /// <summary>
    /// when the machine goes into the state
    /// </summary>
    public virtual void Enter() { }
    /// <summary>
    /// when the machine exits state
    /// </summary>
    public virtual void Exit() { }
    /// <summary>
    /// first frame of enter
    /// </summary>
    public virtual void Ready() { }
}
