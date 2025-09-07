using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/07/2025]
 * [handles inventory for units]
 */

public partial class UnitInventory : Node
{
    [Export] private Unit _unit;
    [Export] private WeaponResource _unarmedResource;

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
    /// equips item to weapon slot
    /// </summary>
    /// <param name="weapon"></param>
    public void EquipWeapon(IInventoryItem weapon)
    {
        if (_equiptWeapon == weapon)
        {
            return;
        }

        WeaponResource newWeapon = weapon as WeaponResource;
        if (newWeapon == null)
        {
            return;
        }
        if (newWeapon.equipableSlot != EquipableSlotEnum.WEAPON)
        {
            return;
        }

        if (!HasEquippedWeapon())
        {
            _equiptWeapon = newWeapon;
            _unit.unitSprite.skin = _unit.unitClass.GetWeaponTexture(_equiptWeapon.weaponType);

            if (HasItemInInventory(newWeapon))
            {
                _inventoryItems.Remove(newWeapon);
            }

            return;
        }
        else
        {
            WeaponResource oldWeapon = _equiptWeapon;
            if (HasItemInInventory(newWeapon))
            {
                if (!CanReplaceInventoryItem(oldWeapon, newWeapon))
                {
                    return;
                }

                _inventoryItems.Remove(newWeapon);
                _inventoryItems.Add(oldWeapon);

                _equiptWeapon = newWeapon;
                _unit.unitSprite.skin = _unit.unitClass.GetWeaponTexture(_equiptWeapon.weaponType);
                return;
            }
            else
            {
                if (!CanAddItemToInventory(oldWeapon))
                {
                    return;
                }

                _inventoryItems.Add(oldWeapon);

                _equiptWeapon = newWeapon;
                _unit.unitSprite.skin = _unit.unitClass.GetWeaponTexture(_equiptWeapon.weaponType);
                return;
            }
        }
    }

    /// <summary>
    /// checks to see if a unit can replace an item in inventory
    /// with a different item
    /// </summary>
    /// <param name="newItem">item replacing</param>
    /// <param name="oldItem">item being replaced</param>
    /// <returns>true if it can be replaced</returns>
    public bool CanReplaceInventoryItem(IInventoryItem newItem, IInventoryItem oldItem)
    {
        if (!_inventoryItems.Contains(oldItem))
        {
            return false;
        }

        return GetCurrentOccupiedInventory() - oldItem.size + newItem.size <= _maxInventorySlots;
    }

    /// <summary>
    /// does unit have weapon equipt
    /// </summary>
    /// <returns></returns>
    public bool HasEquippedWeapon()
    {
        return _equiptWeapon != null;
    }

    /// <summary>
    /// checks if item is in inventory
    /// </summary>
    /// <param name="item">item in inventory</param>
    /// <returns>true if in inventory</returns>
    private bool HasItemInInventory(IInventoryItem item)
    {
        return _inventoryItems.Contains(item);
    }

    //propertieds

    public IInventoryItem[] inventoryItems
    {
        get { return _inventoryItems.ToArray(); }
    }

    public WeaponResource equiptWeapon
    {
        get
        {
            if (!HasEquippedWeapon())
            {
                return _unarmedResource;
            }
            return _equiptWeapon;
        }
    }
}
