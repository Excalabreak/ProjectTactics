using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [06/30/2025]
 * [A priority list for DijkstraPathfinding
 * Ill be honest, there is probably a
 * better way of doing this, i don't want to figure out
 * how to make an adaptable Priority queue right now]
 */

public partial class DijkstraPriorityList : Node
{
    private List<Tuple<Vector2, float>> _priorityList = new List<Tuple<Vector2, float>>();

    /// <summary>
    /// adds and prioritizes list
    /// </summary>
    /// <param name="value">value</param>
    /// <param name="priority">priority</param>
    public void Enqueue(Vector2 value, float priority)
    {
        Tuple<Vector2, float> valuePriorityPair = new Tuple<Vector2, float>(value, priority);

        if (_priorityList.Count == 0)
        {
            _priorityList.Add(valuePriorityPair);
        }

        int loc = _priorityList.Count;
        for (int i = 0; i < _priorityList.Count; i++)
        {
            if (priority < _priorityList[i].Item2)
            {
                loc = i;
                break;
            }
        }

        if (loc == _priorityList.Count)
        {
            _priorityList.Add(valuePriorityPair);
        }
        else
        {
            _priorityList.Insert(loc, valuePriorityPair);
        }
    }

    /// <summary>
    /// changes the priority of a list item
    /// </summary>
    /// <param name="value">value</param>
    /// <param name="newPriority">new priority of value</param>
    public void ChangePriority(Vector2 value, float newPriority)
    {
        int index = _priorityList.FindIndex(t => t.Item1 == value);
        _priorityList.RemoveAt(index);

        Enqueue(value, newPriority);
    }

    /// <summary>
    /// tries to dequeue the first element of array
    /// </summary>
    /// <param name="value">value being dequed</param>
    /// <param name="priority">prio beind dequed</param>
    /// <returns>true if lists can dequeue</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out Vector2 value, [MaybeNullWhen(false)] out float priority)
    {
        if (_priorityList.Count == 0)
        {
            value = default;
            priority = default;
            return false;
        }

        value = _priorityList[0].Item1;
        priority = _priorityList[0].Item2;
        _priorityList.RemoveAt(0);
        return true;
    }

    /// <summary>
    /// returns the count of the list
    /// </summary>
    /// <returns>int of index in priority list</returns>
    public int Count()
    {
        return _priorityList.Count;
    }

    /// <summary>
    /// checks if value is in priority list
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ContainsValue(Vector2 value)
    {
        return _priorityList.Any(m => m.Item1 == value);
    }
}
