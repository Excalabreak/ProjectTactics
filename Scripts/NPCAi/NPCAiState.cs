using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/25/2025]
 * [state for npc ai]
 */

public partial class NPCAiState : Node
{
    public NPCAiStateMachine stateMachine;

    [Export] protected NPCAiState[] _nextPossibleStates;

    /// <summary>
    /// the logic for unit for the turn
    /// </summary>
    public virtual void TurnLogic() { }

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
