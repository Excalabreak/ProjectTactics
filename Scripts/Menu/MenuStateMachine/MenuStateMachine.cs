using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: [YT: Mina Pêcheux]
 * Last Updated: [GD.Print("NO CAMERA STATE FOR " + key);]
 * [script for the menu state machine]
 */

public partial class MenuStateMachine : Node
{
    [Export] public NodePath initialState;
    [Export] private GameBoard _gameBoard;

    private Dictionary<string, MenuState> _states;
    private MenuState _currentState;

    /// <summary>
    /// sets up state machine
    /// </summary>
    public override void _Ready()
    {
        _states = new Dictionary<string, MenuState>();
        foreach (Node node in GetChildren())
        {
            if (node is MenuState s)
            {
                _states[node.Name] = s;

                s.stateMachine = this;
                s.Ready();
                s.Exit();
            }
        }
        _currentState = GetNode<MenuState>(initialState);
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
            GD.Print("NO MENU STATE FOR " + key);
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }

    public MenuState currentState
    {
        get { return _currentState; }
    }

    public GameBoard gameBoard
    {
        get { return _gameBoard; }
    }
}
