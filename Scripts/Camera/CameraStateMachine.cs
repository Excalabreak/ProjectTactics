using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/03/2025]
 * [state machine for camera]
 */

public partial class CameraStateMachine : Node
{
    [Export] private GameBoardCamera _camera;
    [Export] public NodePath initialState;

    private Dictionary<string, CameraState> _states;
    private CameraState _currentState;

    private bool _ignoredReady = false;

    /// <summary>
    /// sets up state machine
    /// </summary>
    public override void _Ready()
    {
        _states = new Dictionary<string, CameraState>();
        foreach (Node node in GetChildren())
        {
            if (node is CameraState s)
            {
                _states[node.Name] = s;

                s.stateMachine = this;
                s.Ready();
                s.Exit();
            }
        }
        _currentState = GetNode<CameraState>(initialState);
        _currentState.Enter();
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

    /// <summary>
    /// indicates it has ignored ready
    /// </summary>
    public void HasIgnoredReady()
    {
        _ignoredReady = true;
    }

    //properties

    public CameraState currentState
    {
        get { return _currentState; }
    }

    public GameBoardCamera camera
    {
        get { return _camera; }
    }

    public bool ignoredReady
    {
        get { return _ignoredReady; }
    }
}
