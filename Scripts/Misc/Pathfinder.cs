using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [04/23/2025]
 * [Finds the path between two points among 
 * walkable cells using the AStar pathfinding algorithm]
 */

public partial class Pathfinder : RefCounted
{
    //take out later when grid is in map manager
    private GridResource _grid;

    private AStar2D _astar = new AStar2D();

    /// <summary>
    /// initiallizes a dictionary for nodes
    /// </summary>
    /// <param name="grid">grid infp</param>
    /// <param name="walkableCells">cells in the map</param>
    public void Init(GridResource grid, Vector2[] walkableCells)
    {
        _grid = grid;

        Dictionary<Vector2, int> cellMapping = new Dictionary<Vector2, int>();
        foreach (Vector2 cell in walkableCells)
        {
            cellMapping[cell] = _grid.AsIndex(cell);
        }

        AddAndConnectPoints(cellMapping);
    }

    /// <summary>
    /// Returns the path found between `start` and `end` as an array of Vector2 coordinates.
    /// </summary>
    /// <param name="start">starting cell</param>
    /// <param name="end">ending cell</param>
    /// <returns>path</returns>
    public Vector2[] CalculatePointPath(Vector2 start, Vector2 end)
    {
        int startIndex = _grid.AsIndex(start);
        int endIndex = _grid.AsIndex(end);

        if (_astar.HasPoint(startIndex) && _astar.HasPoint(endIndex))
        {
            return _astar.GetPointPath(startIndex, endIndex);
        }
        return new Vector2[0];
    }

    /// <summary>
    /// Adds and connects the walkable cells to the Astar2D object
    /// </summary>
    /// <param name="cellMapping">all the cells and their indexes</param>
    private void AddAndConnectPoints(Dictionary<Vector2, int> cellMapping)
    {
        foreach (KeyValuePair<Vector2, int> point in cellMapping)
        {
            _astar.AddPoint(cellMapping[point.Key], point.Key);
        }

        foreach (KeyValuePair<Vector2, int> point in cellMapping)
        {
            foreach (int neighborIndex in FindNeighborIndices(point.Key, cellMapping))
            {
                _astar.ConnectPoints(cellMapping[point.Key], neighborIndex);
            }
        }
    }

    /// <summary>
    /// Returns an array of the `cell`'s connectable neighbors.
    /// </summary>
    /// <param name="cell">cell</param>
    /// <param name="cellMapping">cell mapping</param>
    /// <returns>cell's neighbors</returns>
    private int[] FindNeighborIndices(Vector2 cell, Dictionary<Vector2, int> cellMapping)
    {
        List<int> neighborList = new List<int>();

        foreach (int dir in Enum.GetValues(typeof(Direction)))
        {
            Vector2 neighbor = cell + GetDirection((Direction)dir);

            if (!cellMapping.ContainsKey(neighbor))
            {
                continue;
            }

            if (!_astar.ArePointsConnected(cellMapping[cell], cellMapping[neighbor]))
            {
                neighborList.Add(cellMapping[neighbor]);
            }
        }
        int[] output = neighborList.ToArray();
        return output;
    }

    /// <summary>
    /// returns corresponding vector2
    /// becuase vector2 cant be a const
    /// </summary>
    /// <param name="direction">enum of direction</param>
    /// <returns>vector2 of direction</returns>
    private Vector2 GetDirection(Direction direction)
    {
        Vector2 dir = new Vector2();
        switch (direction)
        {
            case Direction.UP:
                dir = Vector2.Up;
                break;
            case Direction.DOWN:
                dir = Vector2.Down;
                break;
            case Direction.LEFT:
                dir = Vector2.Left;
                break;
            case Direction.RIGHT:
                dir = Vector2.Right;
                break;
        }
        return dir;
    }
}

//enum for directions
public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
