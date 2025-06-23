using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/23/2025]
 * [state for npc ai]
 */

public partial class NPCAiState : Node
{
    public NPCAiStateMachine stateMachine;

    /// <summary>
    /// the logic in this function will determine
    /// which state to transition to
    /// </summary>
    /// <returns>next state</returns>
    public virtual NPCAiState CheckTrigger()
    {
        return this;
    }

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
