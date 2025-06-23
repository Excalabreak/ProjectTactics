using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/23/2025]
 * [idle state for NPCs]
 */

public partial class IdleState : NPCAiState
{
    public override NPCAiState CheckTrigger()
    {
        return this;
    }
}
