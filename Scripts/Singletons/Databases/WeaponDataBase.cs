using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
 * [singleton for weapons]
 */

public partial class WeaponDataBase : Node
{
    private static WeaponDataBase _instance;
    [Export] private WeaponResource[] _weaponResources;

    private Dictionary<string, WeaponResource> _weaponDatabase;

    public override void _Ready()
    {
        if (_instance != null)
        {
            QueueFree();
            return;
        }

        _instance = this;

        _weaponDatabase = new Dictionary<string, WeaponResource>();

        foreach (WeaponResource weapon in _weaponResources)
        {
            if (_weaponDatabase.ContainsKey(weapon.devName))
            {
                continue;
            }

            _weaponDatabase.Add(weapon.devName, weapon);
        }
    }

    /// <summary>
    /// returns an instance of a weapon
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public WeaponResource GetItem(string key)
    {
        return _weaponDatabase[key].Duplicate() as WeaponResource;
    }

    /// <summary>
    /// gets singelton
    /// </summary>
    public static WeaponDataBase Instance
    {
        get { return _instance; }
    }
}
