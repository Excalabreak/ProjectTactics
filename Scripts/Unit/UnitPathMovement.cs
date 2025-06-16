using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/16/2025]
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
        if (_currentDirectionIndex < _pathDirections.Count &&
            _pathFollow.ProgressRatio >= (1f / (float)_pathDirections.Count) * _currentDirectionIndex)
        {
            Vector2 newLoc = _gameBoard.grid.CalculateGridCoordinates(_walkingLocation.GlobalPosition);
            _gameBoard.ChangeUnitLocationData(_unit, newLoc);
            _unit.cell = newLoc;

            _unitDirection.currentFacing = _pathDirections[_currentDirectionIndex];
            _currentDirectionIndex++;

            _gameBoard.UpdateUnitVision(_unit);


            Vector2 nextTile = _unit.cell + DirectionManager.Instance.GetVectorDirection(_unitDirection.currentFacing);
            if (_currentDirectionIndex + 1 <= _pathDirections.Count && _gameBoard.IsOccupied(nextTile))
            {
                StopWalk(grid, newLoc);
            }
        }

        if (_pathFollow.ProgressRatio >= 1f)
        {
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
    /// </summary>
    /// <param name="path">array of grid coordinates</param>
    /// <param name="grid">grid infp</param>
    public void SetWalkPath(Vector2[] path, GridResource grid)
    {
        if (path.Length <= 0)
        {
            return;
        }
        if (Curve == null)
        {
            Curve = new Curve2D();
        }

        Curve.AddPoint(Vector2.Zero);
        foreach (Vector2 point in path)
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
        _unit.targetCell = path[path.Length - 1];
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
