using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author:
 * YT: Bramwell: Camera
 * GDForum: MiltonVines: Lerp
 * 
 * Last Updated: [08/04/2025]
 * [script for the camera]
 */

public partial class GameBoardCamera : Camera2D
{
    [Export] private GameBoard _gameBoard;
    [Export] private CameraStateMachine _cameraStateMachine;
    [Export] private GridCursor _gridCursor;

    private Vector2 _viewportSize;
    private bool _dragCamera = false;
    private bool _isPlayerControllingCamera = true;

    private CameraZoomEnum _currentZoom = CameraZoomEnum.MED_ZOOM;
    private float _targetZoom = 1.0f;
    private const float MIN_ZOOM = 0.75f;
    private const float MED_ZOOM = 1.0f;
    private const float MAX_ZOOM = 1.75f;
    private const float ZOOM_RATE = 8.0f;
    private bool _needZoom = false;

    //have to make my own version of these systems cause godot is weird
    [Export] private int _numOfCellToExtendCamLimits = 3;
    private float _limitLeft = -10000000;
    private float _limitTop = -10000000;
    private float _limitRight = 10000000;
    private float _limitBottom = 10000000;

    [Export] private float _xDragMargin = 0.8f;
    [Export] private float _yDragMargin = 0.65f;

    /// <summary>
    /// sets the limits of the camera
    /// </summary>
    public override void _Ready()
    {
        _viewportSize = GetViewport().GetVisibleRect().Size;

        SetLimits();
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

        if (_isPlayerControllingCamera && 
            _cameraStateMachine.currentState.Name == "CameraFollowCursorState")
        {
            KeepCursorInFrame();
        }
        
        MakeSureCameraIsWithinLimits();
    }

    /// <summary>
    /// handles inputs relating to the camera
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isPlayerControllingCamera)
        {
            return;
        }

        if (_cameraStateMachine.currentState.Name == "CameraClickDragState")
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
                    float halfCamX = _viewportSize.X / Zoom.X / 2;
                    float halfCamY = _viewportSize.Y / Zoom.Y / 2;

                    if (CanHorizontallyMove())
                    {
                        moveCamera.X = Mathf.Clamp(moveCamera.X, _limitLeft + halfCamX, _limitRight - halfCamX);
                    }
                    else
                    {
                        moveCamera.X = _gameBoard.grid.CenterXPos();
                    }

                    if (CanVerticallyMove())
                    {
                        moveCamera.Y = Mathf.Clamp(moveCamera.Y, _limitTop + halfCamY, _limitBottom - halfCamY);
                    }
                    else
                    {
                        moveCamera.Y = _gameBoard.grid.CenterYPos();
                    }
                    Position = moveCamera;
                }
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
    /// this is basically the drag margins camera has built in,
    /// but actually fuckin changes the position of the camera
    /// </summary>
    private void KeepCursorInFrame()
    {
        Vector2 cursorLoc = _gridCursor.GetGlobalTransformWithCanvas().Origin;

        float halfCamX = _viewportSize.X / Zoom.X / 2;
        float halfCamY = _viewportSize.Y / Zoom.Y / 2;

        float xMargin = halfCamX * (1 - _xDragMargin);
        float yMargin = halfCamY * (1 - _yDragMargin);

        float moveX = 0;
        float moveY = 0;
        if (CanHorizontallyMove())
        {
            if (cursorLoc.X < xMargin)
            {
                moveX = cursorLoc.X - xMargin;
            }
            else if (cursorLoc.X > _viewportSize.X - xMargin)
            {
                moveX = cursorLoc.X - (_viewportSize.X - xMargin);
            }
            moveX = Mathf.Clamp(Position.X + moveX, _limitLeft + halfCamX, _limitRight - halfCamY);
        }
        if (CanVerticallyMove())
        {
            if (cursorLoc.Y < yMargin)
            {
                moveY = cursorLoc.Y - yMargin;
            }
            else if (cursorLoc.Y > _viewportSize.Y - yMargin)
            {
                moveY = cursorLoc.Y - (_viewportSize.Y - yMargin);
            }

            moveY = Mathf.Clamp(Position.Y + moveY, _limitLeft + halfCamY, _limitRight - halfCamY);
        }

        Position = new Vector2(moveX, moveY);
    }

    /// <summary>
    /// makes sure camera is within limits
    /// </summary>
    private void MakeSureCameraIsWithinLimits()
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
    /// sets the limits for the game board
    /// </summary>
    private void SetLimits()
    {
        int extendLimitsX = _numOfCellToExtendCamLimits * Mathf.RoundToInt(_gameBoard.grid.cellSize.X);
        int extendLimitsY = _numOfCellToExtendCamLimits * Mathf.RoundToInt(_gameBoard.grid.cellSize.Y);

        _limitLeft = -extendLimitsX;
        _limitTop = -extendLimitsY;
        _limitRight = Mathf.RoundToInt(_gameBoard.grid.cellSize.X) * Mathf.RoundToInt(_gameBoard.grid.gridSize.X) + extendLimitsX;
        _limitBottom = Mathf.RoundToInt(_gameBoard.grid.cellSize.Y) * Mathf.RoundToInt(_gameBoard.grid.gridSize.Y) + extendLimitsY;

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
        float halfCamY = _viewportSize.Y / Zoom.Y / 2;
        return _limitTop + halfCamY < _limitBottom - halfCamY;
    }

    /// <summary>
    /// checks if camera can move horizontally
    /// </summary>
    /// <returns>true if camera can move horizontally</returns>
    private bool CanHorizontallyMove()
    {
        float halfCamX = _viewportSize.X / Zoom.X / 2;
        return _limitLeft + halfCamX < _limitRight - halfCamX;
    }

    /// <summary>
    /// checks if camera is within the X limits
    /// </summary>
    private void MakeSureCamIsWithinXLimit()
    {
        float halfCamX = _viewportSize.X / Zoom.X / 2;

        if (Position.X < _limitLeft + halfCamX)
        {
            Position = new Vector2(_limitLeft + halfCamX, Position.Y);
        }
        else if (Position.X > _limitRight - halfCamX)
        {
            Position = new Vector2(_limitRight - halfCamX, Position.Y);
        }
    }

    /// <summary>
    /// checks if camera is within the Y limits
    /// </summary>
    private void MakeSureCamIsWithinYLimit()
    {
        float halfCamY = _viewportSize.Y / Zoom.Y / 2;

        if (Position.Y < _limitTop + halfCamY)
        {
            Position = new Vector2(Position.X, _limitTop + halfCamY);
        }
        else if (Position.Y > _limitBottom - halfCamY)
        {
            Position = new Vector2(Position.X, _limitBottom - halfCamY);
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

    //properties

    public bool isPlayerControllingCamera
    {
        get { return _isPlayerControllingCamera; }
        set { _isPlayerControllingCamera = value; }
    }

    public CameraStateMachine cameraStateMachine
    {
        get { return _cameraStateMachine; }
    }

    public GridCursor gridCursor
    {
        get { return _gridCursor; }
    }
}
