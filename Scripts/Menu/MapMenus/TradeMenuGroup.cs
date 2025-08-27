using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/27/2025]
 * [group for trade menu for one unit]
 */

public partial class TradeMenuGroup : Node
{
    [Export] private TradeMenu _tradeMenu;

    [Export] private Button _equippedWeaponButton;
    [Export] private Button[] _itemSlotButtons;

    public void SetUpGroup(Unit unit)
    {
        UnitInventory inventory = unit.unitInventory;

        _equippedWeaponButton.Text = "E: " + inventory.equiptWeapon.itemName;
        _tradeMenu.AddToButtonDictionary(_equippedWeaponButton, inventory.equiptWeapon);

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                _itemSlotButtons[i].Text = "---";
                break;
            }

            _itemSlotButtons[i].Text = inventory.inventoryItems[i].itemName;
            _tradeMenu.AddToButtonDictionary(_itemSlotButtons[i], inventory.inventoryItems[i]);
        }
    }
}
