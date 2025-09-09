using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
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
        equiptWeapon = _unit.unitResource.equiptWeapon;

        if (_unit.unitResource.inventoryItems == null)
        {
            return;
        }

        int count = _maxInventorySlots;

        foreach (IInventoryItem item in _unit.unitResource.inventoryItems)
        {
            AddInventoryItem(item);
            count--;
            if (count <= 0)
            {
                break;
            }
        }

        if (count > 0)
        {
            foreach (IInventoryItem item in _unit.unitResource.consumableItems)
            {
                AddInventoryItem(item);
                count--;
                if (count <= 0)
                {
                    break;
                }
            }
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
    /// removes item from inventory
    /// </summary>
    /// <param name="item">item to remove</param>
    public void RemoveInventoryItem(IInventoryItem item)
    {
        if (!_inventoryItems.Contains(item))
        {
            return;
        }

        _inventoryItems.Remove(item);
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
            equiptWeapon = newWeapon;

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

                equiptWeapon = newWeapon;
                return;
            }
            else
            {
                if (!CanAddItemToInventory(oldWeapon))
                {
                    return;
                }

                _inventoryItems.Add(oldWeapon);

                equiptWeapon = newWeapon;
                return;
            }
        }
    }

    public void EquipWeaponFromInventory(IInventoryItem weapon)
    {
        if (_equiptWeapon == weapon)
        {
            return;
        }
        if (!_inventoryItems.Contains(weapon))
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
    }

    /// <summary>
    /// attempts to dequip the currently equipt weapon
    /// and sends it to inventory
    /// </summary>
    public void DequipWeaponToInventory()
    {
        if (!HasEquippedWeapon())
        {
            return;
        }
        if (!CanAddItemToInventory(_equiptWeapon))
        {
            return;
        }

        _inventoryItems.Add(_equiptWeapon);

        equiptWeapon = null;
    }

    /// <summary>
    /// removes equipped weapon
    /// </summary>
    public void RemoveEquippedWeapon()
    {
        if (!HasEquippedWeapon())
        {
            return;
        }

        equiptWeapon = null;
    }

    /// <summary>
    /// attempts to use Item
    /// </summary>
    /// <param name="consumable"></param>
    public void UseItem(IInventoryItem consumable)
    {
        IUseable item = consumable as IUseable;

        if (item == null)
        {
            return;
        }

        item.OnUse(_unit);
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

        private set
        {
            _equiptWeapon = value;
            _unit.unitSprite.skin = _unit.unitClass.GetWeaponTexture(equiptWeapon.weaponType);
        }
    }
}
