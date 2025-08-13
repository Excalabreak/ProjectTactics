using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/13/2025]
 * [static data base for weapons]
 */

public partial class WeaponStaticData : StaticData
{
    private static WeaponStaticData _instance;

    /// <summary>
    /// overrides the data path
    /// </summary>
    public override void _Ready()
    {
        if (_instance != null)
        {
            QueueFree();
            return;
        }

        _instance = this;

        _dataFilePath = "res://StaticData/WeaponData.json";
        base._Ready();
    }

    /// <summary>
    /// gets singelton
    /// </summary>
    public static WeaponStaticData Instance
    {
        get { return _instance; }
    }
}
