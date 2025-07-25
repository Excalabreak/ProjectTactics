using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/25/2025]
 * [state machine for npc AI]
 */

public partial class NPCAiStateMachine : Node
{
    [Signal] public delegate void UnitFinishedEventHandler();

    [Export] public Unit _unit;
    [Export] public NodePath initialState;

    private Dictionary<string, NPCAiState> _states;
    private NPCAiState _currentState;

    private GameBoard _gameBoard;

    private bool _needsAwait = false;

    /// <summary>
    /// sets up state machine
    /// </summary>
    public override void _Ready()
    {
        _unit.CurrentGameBoard += SetGameBoard;
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
    public void DoLogic()
    {
        _currentState.TurnLogic();
    }

    /// <summary>
    /// signals that the unit is finished
    /// </summary>
    public void UnitFinsh()
    {
        EmitSignal("UnitFinished");
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
    /// sets _needsAwait to false
    /// </summary>
    public void ResetNeedsAwait()
    {
        _needsAwait = false;
    }

    /// <summary>
    /// changes bool to signal that the
    /// logic needs await
    /// </summary>
    public void LogicNeedsAwait()
    {
        _needsAwait = true;
    }

    /// <summary>
    /// unsubs from event
    /// </summary>
    public override void _ExitTree()
    {
        _unit.CurrentGameBoard -= SetGameBoard;
    }

    /// <summary>
    /// sets _gameBoard
    /// </summary>
    /// <param name="gameBoard">v</param>
    private void SetGameBoard(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;
    }

    public bool needsAwait
    {
        get { return _needsAwait; }
    }

    public NPCAiState currentState
    {
        get { return _currentState; }
    }

    public GameBoard gameBoard
    {
        get { return _gameBoard; }
    }

    public Unit unit
    {
        get { return _unit; }
    }
}
