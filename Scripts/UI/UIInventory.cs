using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/25/2025]
 * [displays a unit inventory]
 */

public partial class UIInventory : Control
{
    [Export] private Label _equippedLable;
    [Export] private Label[] _itemLables;

    /// <summary>
    /// shows the unit inventory
    /// </summary>
    /// <param name="equippedWeapon">weapon that is equipped</param>
    /// <param name="items">items in inventory</param>
    public void ShowInventory(UnitInventory inventory)
    {
        _equippedLable.Text = "Equipped: " + inventory.equiptWeapon.itemName;

        IInventoryItem[] items = inventory.inventoryItems;

        if (items.Length > _itemLables.Length)
        {
            GD.Print("too many items for inventory");
        }

        for (int i = 0; i < _itemLables.Length; i++)
        {
            if (items.Length < i + 1)
            {
                _itemLables[i].Text = "";
            }
            else
            {
                _itemLables[i].Text = items[i].itemName;
            }
        }

        this.Visible = true;
    }

    /// <summary>
    /// hides unit inventory
    /// </summary>
    public void HideInventory()
    {
        this.Visible = false;
    }
}
