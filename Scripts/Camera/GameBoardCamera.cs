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
    [Export] private CameraStateMachine _cameraStateMachine;
    [Export] private RemoteTransform2D _remoteTransform;

    private bool _dragCamera = false;

    private CameraZoomEnum _currentZoom = CameraZoomEnum.MED_ZOOM;
    private float _targetZoom = 1.0f;
    private const float MIN_ZOOM = 0.5f;
    private const float MED_ZOOM = 1.0f;
    private const float MAX_ZOOM = 2.0f;
    private const float ZOOM_RATE = 8.0f;

    /// <summary>
    /// zooms in to the target zoom
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        Zoom = Lerp(Zoom, _targetZoom * Vector2.One, ZOOM_RATE * ((float)delta));
        SetPhysicsProcess(!Mathf.IsEqualApprox(Zoom.X, _targetZoom));
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

        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;

            if (_dragCamera)
            {
                Position -= mouseMotion.Relative / Zoom;
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
        
        ZoomToCurrent();
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

        ZoomToCurrent();
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

        ZoomToCurrent();
    }

    /// <summary>
    /// sets the target zoom to the current zoom and starts zooming camera
    /// </summary>
    private void ZoomToCurrent()
    {
        _targetZoom = GetZoom(_currentZoom);
        SetPhysicsProcess(true);
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

    public CameraStateMachine cameraStateMachine
    {
        get { return _cameraStateMachine; }
    }

    public RemoteTransform2D remoteTransform
    {
        get { return _remoteTransform; }
    }
}
