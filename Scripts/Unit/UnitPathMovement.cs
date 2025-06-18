using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/18/2025]
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
    public void WalkUnit(float delta, GridResource grid, Vector2 cell)
    {
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
            _gameBoard.ChangeUnitLocationData(_unit, newLoc);
            _unit.cell = newLoc;

            _gameBoard.UpdateUnitVision(_unit);


            //checks next tile
            Vector2 nextTile = newLoc + DirectionManager.Instance.GetVectorDirection(_pathDirections[_currentDirectionIndex]);
            if (!_gameBoard.CheckCanPass(_unit, nextTile))
            {
                //check future tiles here
                StopWalk(grid, _unit.cell);
            }

            
            if (_currentDirectionIndex != 0)
            {
                _unit.unitStats.UseMove(_gameBoard.map.GetTileMoveCost(newLoc));
            }

            _currentDirectionIndex++;
        }

        if (_pathFollow.ProgressRatio >= 1f)
        {
            _unit.unitStats.UseMove(_gameBoard.map.GetTileMoveCost(cell));
            StopWalk(grid, cell);
        }
    }

    /// <summary>
    /// stops the unit from walking
    /// </summary>
    /// <param name="grid">grid info</param>
    /// <param name="cell">where to stop unit</param>
    private void StopWalk(GridResource grid, Vector2 cell)
    {
        this._isWalking = false;
        _pathFollow.Progress = 0f;

        _gameBoard.ChangeUnitLocationData(_unit, cell);
        _unit.cell = cell;
        _unit.Position = grid.CalculateMapPosition(cell);

        _gameBoard.UpdateUnitVision(_unit);

        Curve.ClearPoints();
        _pathDirections = new List<DirectionEnum>();
        _currentDirectionIndex = 0;

        EmitSignal("WalkFinished");
    }

    /// <summary>
    /// sets the walk path based on the coordinates
    /// 
    /// note, grid might need to be taken out and put in a map manager
    /// NOTE: PATH INCLUDES THE INDEX THE UNIT IS STANDING ON 
    /// </summary>
    /// <param name="path">array of grid coordinates</param>
    /// <param name="grid">grid infp</param>
    public void SetWalkPath(Vector2[] path, GridResource grid)
    {
        if (path.Length <= 1)
        {
            StopWalk(_gameBoard.grid, _unit.cell);
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

        //adds the walkable tiles to the list
        //(i feel like there is a better way of doing this
        //but can't find it)
        if (lastStandableIndex > 0)
        {
            for (int i = 0; i <= lastStandableIndex; i++)
            {
                walkablePath.Add(path[i]);
            }
        }
        

        Curve.AddPoint(Vector2.Zero);
        foreach (Vector2 point in walkablePath)
        {
            if (grid.CalculateMapPosition(point) - _unit.Position != Curve.GetPointPosition(Curve.PointCount - 1))
            {
                Vector2 nextPoint = grid.CalculateMapPosition(point) - _unit.Position;
                Vector2 lastPoint = Curve.GetPointPosition(Curve.PointCount - 1);

                //checks if point has a triggerable
                Curve.AddPoint(nextPoint);

                if (Mathf.RoundToInt(nextPoint.Y) != Mathf.RoundToInt(lastPoint.Y))
                {
                    if (Mathf.RoundToInt(nextPoint.Y) < Mathf.RoundToInt(lastPoint.Y))
                    {
                        _pathDirections.Add(DirectionEnum.UP);
                    }
                    else
                    {
                        _pathDirections.Add(DirectionEnum.DOWN);
                    }
                }
                else
                {
                    if (Mathf.RoundToInt(nextPoint.X) < Mathf.RoundToInt(lastPoint.X))
                    {
                        _pathDirections.Add(DirectionEnum.LEFT);
                    }
                    else
                    {
                        _pathDirections.Add(DirectionEnum.RIGHT);
                    }
                }
            }
        }
        _unit.targetCell = walkablePath[lastStandableIndex];
        isWalking = true;
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
