using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/02/2025]
 * [state for npc ai]
 */

public partial class NPCAiState : Node
{
    public NPCAiStateMachine stateMachine;

    /// <summary>
    /// the logic for unit for the turn
    /// </summary>
    public virtual void TurnLogic() { }

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
