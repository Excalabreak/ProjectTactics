using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/23/2025]
 * [state machine for npc AI]
 */

public partial class NPCAiStateMachine : Node
{
    [Export] public NodePath initialState;

    private Dictionary<string, NPCAiState> _states;
    private NPCAiState _currentState;

    /// <summary>
    /// sets up state machine
    /// </summary>
    public override void _Ready()
    {
        _states = new Dictionary<string, NPCAiState>();
        foreach (Node node in GetChildren())
        {
            if (node is NPCAiState s)
            {
                _states[node.Name] = s;

                s.stateMachine = this;
                s.Ready();
                s.Exit();
            }
        }
        _currentState = GetNode<NPCAiState>(initialState);
        _currentState.Enter();
    }

    /// <summary>
    /// runs logic of current state to
    /// determine what to do and check for switching
    /// states
    /// </summary>
    public void DoTurn()
    {
        NPCAiState nextState = _currentState.CheckTrigger();
        if (_currentState != nextState)
        {
            TransitionTo(nextState.Name);
        }
    }

    /// <summary>
    /// transitions between states
    /// </summary>
    /// <param name="key">key of state</param>
    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _currentState == _states[key])
        {
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }

    public NPCAiState currentState
    {
        get { return _currentState; }
    }
}
