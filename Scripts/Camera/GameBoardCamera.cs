using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author:
 * YT: Bramwell: Camera
 * GDForum: MiltonVines: Lerp
 * 
 * Last Updated: [08/03/2025]
 * [script for the camera]
 */

public partial class GameBoardCamera : Camera2D
{
    [Export] private GameBoard _gameBoard;
    [Export] private CameraStateMachine _cameraStateMachine;
    [Export] private GridCursor _gridCursor;

    [Export] private int _numOfCellToExtendCamLimits = 3;

    private Vector2 _viewportSize;
    private bool _dragCamera = false;

    private CameraZoomEnum _currentZoom = CameraZoomEnum.MED_ZOOM;
    private float _targetZoom = 1.0f;
    private const float MIN_ZOOM = 0.5f;
    private const float MED_ZOOM = 1.0f;
    private const float MAX_ZOOM = 2.0f;
    private const float ZOOM_RATE = 8.0f;
    private bool _needZoom = false;

    private float _limitLeft = -10000000;
    private float _limitTop = -10000000;
    private float _limitRight = 10000000;
    private float _limitBottom = 10000000;

    /// <summary>
    /// sets the limits of the camera
    /// </summary>
    public override void _Ready()
    {
        _viewportSize = GetViewport().GetVisibleRect().Size;

        int extendLimitsX = _numOfCellToExtendCamLimits * Mathf.RoundToInt(_gameBoard.grid.cellSize.X);
        int extendLimitsY = _numOfCellToExtendCamLimits * Mathf.RoundToInt(_gameBoard.grid.cellSize.X);

        _limitLeft = -extendLimitsX;
        _limitTop = -extendLimitsY;
        _limitRight = Mathf.RoundToInt(_gameBoard.grid.cellSize.X) * Mathf.RoundToInt(_gameBoard.grid.gridSize.X) + extendLimitsX;
        _limitBottom = Mathf.RoundToInt(_gameBoard.grid.cellSize.Y) * Mathf.RoundToInt(_gameBoard.grid.gridSize.Y) + extendLimitsY;
    }

    /// <summary>
    /// zooms in to the target zoom
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        if (_needZoom)
        {
            ZoomToTarget((float)delta);
        }

        if (_cameraStateMachine.currentState.Name == "CameraClickDragState")
        {
            MakeSureCameraIsWithinLimits();
        }
    }

    /// <summary>
    /// handles inputs relating to the camera
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("DragCamera"))
        {
            _dragCamera = true;
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionReleased("DragCamera"))
        {
            _dragCamera = false;
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (_dragCamera)
            {
                Vector2 moveCamera = Position - mouseMotion.Relative / Zoom;
                float x = _viewportSize.X / Zoom.X / 2;
                float y = _viewportSize.Y / Zoom.Y / 2;

                if (CanHorizontallyMove())
                {
                    moveCamera.X = Mathf.Clamp(moveCamera.X, _limitLeft + x, _limitRight - x);
                }
                else
                {
                    moveCamera.X = _gameBoard.grid.CenterXPos();
                }

                if (CanVerticallyMove())
                {
                    moveCamera.Y = Mathf.Clamp(moveCamera.Y, _limitTop + y, _limitBottom - y);
                }
                else
                {
                    moveCamera.Y = _gameBoard.grid.CenterYPos();
                }
                Position = moveCamera;
            }
        }

        if (@event.IsActionPressed("ZoomIn"))
        {
            ZoomIn();
            GetViewport().SetInputAsHandled();
        }
        if (@event.IsActionPressed("ZoomOut"))
        {
            ZoomOut();
            GetViewport().SetInputAsHandled();
        }
        if (@event.IsActionPressed("CycleZoom"))
        {
            CycleZoom();
            GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>
    /// makes sure camera is within limits
    /// </summary>
    public void MakeSureCameraIsWithinLimits()
    {
        bool handledX = false;
        bool handledY = false;

        if (!CanHorizontallyMove())
        {
            Position = new Vector2(_gameBoard.grid.CenterXPos(), Position.Y);
            handledX = true;
        }
        if (!CanVerticallyMove())
        {
            Position = new Vector2(Position.X, _gameBoard.grid.CenterYPos());
            handledY = true;
        }

        if (!handledX)
        {
            MakeSureCamIsWithinXLimit();
        }

        if (!handledY)
        {
            MakeSureCamIsWithinYLimit();
        }
    }

    /// <summary>
    /// zooms camera using lerp
    /// </summary>
    /// <param name="delta"></param>
    private void ZoomToTarget(float delta)
    {
        Zoom = Lerp(Zoom, _targetZoom * Vector2.One, ZOOM_RATE * delta);
        _needZoom = !Mathf.IsEqualApprox(Zoom.X, _targetZoom);
    }

    /// <summary>
    /// zooms in camera
    /// </summary>
    private void ZoomIn()
    {
        int target = (int)_currentZoom;
        if (target < (int)CameraZoomEnum.MAX_ZOOM)
        {
            target++;
        }

        _currentZoom = (CameraZoomEnum)target;
        
        StartCurrentZoom();
    }

    /// <summary>
    /// zooms out camera
    /// </summary>
    private void ZoomOut()
    {
        int target = (int)_currentZoom;
        if (target > (int)CameraZoomEnum.MIN_ZOOM)
        {
            target--;
        }

        _currentZoom = (CameraZoomEnum)target;

        StartCurrentZoom();
    }

    /// <summary>
    /// zooms but cycles between the zoom states
    /// </summary>
    private void CycleZoom()
    {
        int target = (int)_currentZoom;
        target++;
        if (target > (int)CameraZoomEnum.MAX_ZOOM)
        {
            target = (int)CameraZoomEnum.MIN_ZOOM;
        }

        _currentZoom = (CameraZoomEnum)target;

        StartCurrentZoom();
    }

    /// <summary>
    /// sets the target zoom to the current zoom and starts zooming camera
    /// </summary>
    private void StartCurrentZoom()
    {
        _targetZoom = GetZoom(_currentZoom);
        _needZoom = true;
    }

    /// <summary>
    /// returns the float for the needed zoom
    /// </summary>
    /// <param name="zoom">zoom state</param>
    /// <returns>float of zoom</returns>
    private float GetZoom(CameraZoomEnum zoom)
    {
        switch (zoom)
        {
            case CameraZoomEnum.MIN_ZOOM:
                return MIN_ZOOM;
            case CameraZoomEnum.MED_ZOOM:
                return MED_ZOOM;
            case CameraZoomEnum.MAX_ZOOM:
                return MAX_ZOOM;
            default:
                break;
        }
        return MED_ZOOM;
    }

    /// <summary>
    /// checks if camera can move vertically
    /// </summary>
    /// <returns>true if camera can move vertically</returns>
    private bool CanVerticallyMove()
    {
        float y = _viewportSize.Y / Zoom.Y / 2;
        return _limitTop + y < _limitBottom - y;
    }

    /// <summary>
    /// checks if camera can move horizontally
    /// </summary>
    /// <returns>true if camera can move horizontally</returns>
    private bool CanHorizontallyMove()
    {
        float x = _viewportSize.X / Zoom.X / 2;
        return _limitLeft + x < _limitRight - x;
    }

    /// <summary>
    /// checks if camera is within the X limits
    /// </summary>
    private void MakeSureCamIsWithinXLimit()
    {
        float x = _viewportSize.X / Zoom.X / 2;

        if (Position.X < _limitLeft + x)
        {
            Position = new Vector2(_limitLeft + x, Position.Y);
        }
        else if (Position.X > _limitRight - x)
        {
            Position = new Vector2(_limitRight - x, Position.Y);
        }
    }

    /// <summary>
    /// checks if camera is within the Y limits
    /// </summary>
    private void MakeSureCamIsWithinYLimit()
    {
        float y = _viewportSize.Y / Zoom.Y / 2;

        if (Position.Y < _limitTop + y)
        {
            Position = new Vector2(Position.X, _limitTop + y);
        }
        else if (Position.Y > _limitBottom - y)
        {
            Position = new Vector2(Position.X, _limitBottom - y);
        }
    }

    /// <summary>
    /// lerp function for float
    /// </summary>
    /// <param name="firstFloat">first float</param>
    /// <param name="secondFloat">end float</param>
    /// <param name="by">how far in to the lerp it is</param>
    /// <returns>float</returns>
    private float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    /// <summary>
    /// lerp function for Vector2
    /// </summary>
    /// <param name="firstVector">first Vector2</param>
    /// <param name="secondVector">end Vector2</param>
    /// <param name="by">how far in to the lerp it is</param>
    /// <returns>current Vector2</returns>
    private Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by)
    {
        float retX = Lerp(firstVector.X, secondVector.Y, by);
        float retY = Lerp(firstVector.X, secondVector.Y, by);
        return new Vector2(retX, retY);
    }

    /// <summary>
    /// attaches this camera to the cursor
    /// </summary>
    public void AttachToCursor()
    {
        ResetSmoothing();
        GD.Print(GlobalPosition);
        GD.Print(GetScreenCenterPosition());
        Reparent(_gridCursor, true);

        Position = Vector2.Zero;
    }

    /// <summary>
    /// detaches this camera from cursor
    /// </summary>
    public void DetachFromCursor()
    {
        ResetSmoothing();
        GD.Print(GlobalPosition);
        GD.Print(GetScreenCenterPosition());
        Reparent(_gameBoard, true);
    }

    //properties

    public CameraStateMachine cameraStateMachine
    {
        get { return _cameraStateMachine; }
    }

    public GridCursor gridCursor
    {
        get { return _gridCursor; }
    }
}
