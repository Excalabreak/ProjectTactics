using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Mina Pêcheux]
 * Last Updated: [07/13/2025]
 * [script for the menu state]
 */

public partial class MenuState : Node
{
    public MenuStateMachine stateMachine;

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

    /// <summary>
    /// what happens with cursor clicks while in this state
    /// </summary>
    /// <param name="cell">cell</param>
    public virtual void OnCursorAccept(Vector2 cell) { }

    /// <summary>
    /// what happens with cursor movements while in this state
    /// </summary>
    /// <param name="cell">cell</param>
    public virtual void OnCursorMove(Vector2 newCell) { }

    /// <summary>
    /// what happens with cursor declines while in this state
    /// </summary>
    public virtual void OnCursorDecline() { }

    /// <summary>
    /// when the cursor is on top of a cell
    /// </summary>
    /// <param name="cell">cell</param>
    public virtual void OnHover(Vector2 cell) { }
}
