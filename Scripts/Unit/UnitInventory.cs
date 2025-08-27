using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/26/2025]
 * [handles inventory for units]
 */

public partial class UnitInventory : Node
{
    [Export] private Unit _unit;
    [Export] private WeaponResource _unarmmedResource;

    //[Export] private int _maxHandSlots = 2;
    [Export] private int _maxInventorySlots = 3;

    private WeaponResource _equiptWeapon;
    private List<IInventoryItem> _inventoryItems = new List<IInventoryItem>();

    public override void _Ready()
    {
        _equiptWeapon = _unit.unitResource.equiptWeapon;

        if (_unit.unitResource.inventoryItems == null)
        {
            return;
        }

        foreach (IInventoryItem item in _unit.unitResource.inventoryItems)
        {
            AddInventoryItem(item);
        }
        /*
        GD.Print(_unit.Name);

        foreach (IInventoryItem item in _inventoryItems)
        {
            GD.Print(item);
        }*/
    }
    
    /// <summary>
    /// adds item into inventory if possible
    /// </summary>
    /// <param name="item">item</param>
    public void AddInventoryItem(IInventoryItem item)
    {
        if (!CanAddItemToInventory(item))
        {
            return;
        }

        _inventoryItems.Add(item);
    }

    /// <summary>
    /// checks if item can be added to inventory
    /// </summary>
    /// <param name="item">item to add</param>
    /// <returns>true if item can be added</returns>
    public bool CanAddItemToInventory(IInventoryItem item)
    {
        if (item == null)
        {
            return false;
        }

        return GetCurrentOccupiedInventory() + item.size <= _maxInventorySlots;
    }

    /// <summary>
    /// gets the amount of inventory slots
    /// used by items
    /// </summary>
    /// <returns>occupied inventory</returns>
    public int GetCurrentOccupiedInventory()
    {
        if (_inventoryItems.Count <= 0)
        {
            return 0;
        }

        int output = 0;
        foreach (IInventoryItem item in _inventoryItems)
        {
            output += item.size;
        }
        return output;
    }

    /// <summary>
    /// does unit have weapon equipt
    /// </summary>
    /// <returns></returns>
    public bool HasEquippedWeapon()
    {
        return _equiptWeapon == null;
    }

    public IInventoryItem[] inventoryItems
    {
        get { return _inventoryItems.ToArray(); }
    }

    public WeaponResource equiptWeapon
    {
        get
        {
            if (HasEquippedWeapon())
            {
                return _unarmmedResource;
            }
            return _equiptWeapon;
        }
    }
}
