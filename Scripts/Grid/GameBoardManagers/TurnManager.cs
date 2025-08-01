using Godot;
using System;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/01/2025]
 * [manages the turn order of the game]
 */

public partial class TurnManager : Node
{
    //needed references
    [Export] private UnitManager _unitManager;

    [Export] private UnitGroupEnum _startingGroup = UnitGroupEnum.PLAYER;
    private UnitGroupEnum[] _unitGroupTurns;
    private int _turnIndex;

    /// <summary>
    /// sets the turn index
    /// </summary>
    public override void _Ready()
    {
        _unitGroupTurns = _unitManager.GetAllUnitGroupEnums();

        _turnIndex = 0;
        if (_unitGroupTurns.Contains(_startingGroup))
        {
            _turnIndex = Array.FindIndex(_unitGroupTurns, group => _startingGroup == group);
        }
    }

    /// <summary>
    /// changes who's turn it is
    /// </summary>
    public void NextTurn()
    {
        _turnIndex++;
        if (_turnIndex == _unitGroupTurns.Length)
        {
            _turnIndex = 0;
        }
    }

    /// <summary>
    /// gets which group's turn it is
    /// </summary>
    public UnitGroupEnum currentTurn
    {
        get { return _unitGroupTurns[_turnIndex]; }
    }
}
