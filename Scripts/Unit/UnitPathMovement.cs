using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/25/2025]
 * [moves sprite through path]
 */

public partial class UnitPathMovement : Path2D
{
    private GameBoard _gameBoard;
    [Export] private Unit _unit;

    [Export] private RemoteTransform2D _walkingLocation;

    //path2d
    [Signal] public delegate void WalkFinishedEventHandler();
    [Export] private PathFollow2D _pathFollow;
    private bool _isWalking = false;
    private bool _delayStop = false;
    private bool _hasdelayStop = false;

    //might need to put in a settings
    [Export] private float _moveSpeed = 600f;

    //direction (this seems quick and dirty)
    [Export] private UnitDirection _unitDirection;
    private List<DirectionEnum> _pathDirections = new List<DirectionEnum>();
    private int _currentDirectionIndex = 0;

    public override void _Ready()
    {
        _unit.CurrentGameBoard += SetGameBoard;
        Curve = new Curve2D();
    }

    /// <summary>
    /// moves the sprite of the unit every frame when on route
    /// 
    /// note, grid might need to be taken out and put in a map manager
    /// </summary>
    /// <param name="delta">time between frames</param>
    /// <param name="grid">grid info</param>
    /// <param name="cell">final pos of unit</param>
    public void WalkUnit(float delta, Vector2 cell)
    {
        if (_delayStop)
        {
            if (_hasdelayStop)
            {
                _delayStop = false;
                _hasdelayStop = false;
                StopWalk(_unit.cell);
            }
            else
            {
                _hasdelayStop = true;
            }
            return;
        }
        _pathFollow.Progress += _moveSpeed * delta;

        //this consequncly checks every tile the unit walks on
        //so this is where units update which tile they are on
        //this is probably a bad way of doing this...OH WELL
        if (_pathFollow.ProgressRatio < 1f &&
            _currentDirectionIndex < _pathDirections.Count &&
            _pathFollow.ProgressRatio >= (1f / (float)_pathDirections.Count) * _currentDirectionIndex)
        {
            _unitDirection.currentFacing = _pathDirections[_currentDirectionIndex];

            Vector2 newLoc = _gameBoard.grid.CalculateGridCoordinates(_walkingLocation.GlobalPosition);
            _unit.cell = newLoc;

            _gameBoard.MovingUnitVisionUpdate(_unit, newLoc);
            _gameBoard.UpdateUnitVision(_unit);

            //checks next tile
            Vector2 nextTile = newLoc + DirectionManager.Instance.GetVectorDirection(_pathDirections[_currentDirectionIndex]);
            if (!_gameBoard.CheckCanPass(_unit, nextTile))
            {
                //check future tiles here
                StopWalk(_unit.cell);
            }

            
            if (_currentDirectionIndex != 0)
            {
                _unit.unitActionEconomy.UseMove(_gameBoard.map.GetTileMoveCost(newLoc));
            }

            _currentDirectionIndex++;
        }

        if (_pathFollow.ProgressRatio >= 1f)
        {
            _unit.unitActionEconomy.UseMove(_gameBoard.map.GetTileMoveCost(cell));
            StopWalk(cell);
        }
    }

    /// <summary>
    /// stops the unit from walking
    /// </summary>
    /// <param name="cell">where to stop unit</param>
    private void StopWalk(Vector2 cell)
    {
        this.isWalking = false;
        _pathFollow.Progress = 0f;

        _unit.cell = cell;
        _unit.Position = _gameBoard.grid.CalculateMapPosition(cell);
        _gameBoard.AddUnitLocation(_unit);
        _gameBoard.AddKnownUnitLocation(_unit);

        _gameBoard.UpdateUnitVision(_unit);

        Curve.ClearPoints();
        _pathDirections = new List<DirectionEnum>();
        _currentDirectionIndex = 0;

        _unit.unitActionEconomy.CheckMoveAction();

        EmitSignal("WalkFinished");
    }

    /// <summary>
    /// sets the walk path based on the coordinates
    /// 
    /// note, grid might need to be taken out and put in a map manager
    /// NOTE: PATH INCLUDES THE INDEX THE UNIT IS STANDING ON 
    /// </summary>
    /// <param name="path">array of grid coordinates</param>
    public void SetWalkPath(Vector2[] path)
    {
        if (path.Length <= 1)
        {
            StopWalk(_unit.cell);
            return;
        }
        if (Curve == null)
        {
            Curve = new Curve2D();
        }
        

        //check path here
        List<Vector2> walkablePath = new List<Vector2>();
        int lastStandableIndex = 0;
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == _unit.cell)
            {
                lastStandableIndex = i;
                continue;
            }
            if (!_gameBoard.CheckCanPass(_unit, path[i]))
            {
                break;
            }
            if (_gameBoard.IsOccupied(path[i]))
            {
                continue;
            }

            lastStandableIndex = i;
        }

        //if completely blocked, do this instead
        if (lastStandableIndex <= 0)
        {
            _unitDirection.currentFacing = GetNextDirEnum(path[1], path[0]);
            isWalking = true;
            _delayStop = true;
            _unit.targetCell = _unit.cell;
            return;
        }

        //adds the walkable tiles to the list
        //(i feel like there is a better way of doing this
        //but can't find it)
        for (int i = 0; i <= lastStandableIndex; i++)
        {
            walkablePath.Add(path[i]);
        }

        Curve.AddPoint(Vector2.Zero);
        foreach (Vector2 point in walkablePath)
        {
            if (_gameBoard.grid.CalculateMapPosition(point) - _unit.Position != Curve.GetPointPosition(Curve.PointCount - 1))
            {
                Vector2 nextPoint = _gameBoard.grid.CalculateMapPosition(point) - _unit.Position;
                Vector2 lastPoint = Curve.GetPointPosition(Curve.PointCount - 1);

                //checks if point has a triggerable
                Curve.AddPoint(nextPoint);

                _pathDirections.Add(GetNextDirEnum(nextPoint, lastPoint));
            }
        }
        _unit.targetCell = walkablePath[lastStandableIndex];
        isWalking = true;
        _gameBoard.RemoveUnitLocation(_unit);
        _gameBoard.RemoveKnownUnitLocation(_unit);
    }

    /// <summary>
    /// gets the direction from last point to next point
    /// 
    /// NOTE: coordinates have to be next to each other
    /// </summary>
    /// <param name="nextPoint">point next to the starting point</param>
    /// <param name="lastPoint">starting point</param>
    /// <returns>Direction Enum of the 2 points</returns>
    private DirectionEnum GetNextDirEnum(Vector2 nextPoint, Vector2 lastPoint)
    {
        if (Mathf.RoundToInt(nextPoint.Y) != Mathf.RoundToInt(lastPoint.Y))
        {
            if (Mathf.RoundToInt(nextPoint.Y) < Mathf.RoundToInt(lastPoint.Y))
            {
                return DirectionEnum.UP;
            }
            else
            {
                return DirectionEnum.DOWN;
            }
        }
        else
        {
            if (Mathf.RoundToInt(nextPoint.X) < Mathf.RoundToInt(lastPoint.X))
            {
                return DirectionEnum.LEFT;
            }
            else
            {
                return DirectionEnum.RIGHT;
            }
        }
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

    //properties
    public bool isWalking
    {
        set
        {
            _isWalking = value;
            _unit.unitCanWalk = value;
        }
        get { return _isWalking; }
    }
}
