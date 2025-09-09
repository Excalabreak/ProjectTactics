using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
 * [database for consumables]
 */

public partial class ConsumableDataBase : Node
{
    private static ConsumableDataBase _instance;
    [Export] private ConsumableItemResource[] _consumableResources;

    private Dictionary<string, ConsumableItemResource> _consumableDatabase;

    public override void _Ready()
    {
        if (_instance != null)
        {
            QueueFree();
            return;
        }

        _instance = this;

        _consumableDatabase = new Dictionary<string, ConsumableItemResource>();

        foreach (ConsumableItemResource item in _consumableResources)
        {
            if (_consumableDatabase.ContainsKey(item.devName))
            {
                continue;
            }
            GD.Print(item.devName + ": " + item.itemName);
            _consumableDatabase.Add(item.devName, item);
        }
    }

    /// <summary>
    /// returns an instance of a weapon
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ConsumableItemResource GetItem(string key)
    {
        return _consumableDatabase[key].Duplicate() as ConsumableItemResource;
    }

    /// <summary>
    /// gets singelton
    /// </summary>
    public static ConsumableDataBase Instance
    {
        get { return _instance; }
    }
}
