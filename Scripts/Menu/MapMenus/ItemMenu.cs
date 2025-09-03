using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/03/2025]
 * [script for the menu]
 */

public partial class ItemMenu : BaseMenu
{
    [Export] private Button _equippedWeaponButton;
    [Export] private Button[] _itemSlotButtons;

    private Dictionary<Button, IInventoryItem> _inventorySlots;

    /// <summary>
    /// calls to set the labels for buttons
    /// </summary>
    /// <param name="inventory">inventory for unit</param>
    public void SetUpItemSlots(UnitInventory inventory)
    {
        _equippedWeaponButton.Text = "E: " + inventory.equiptWeapon.itemName;
        _inventorySlots.Add(_equippedWeaponButton, inventory.equiptWeapon);
        //just a note on how to connect a button
        //_equippedWeaponButton.Connect(Button.SignalName.Pressed, Callable.From(OnCancelButtonPress));

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                _itemSlotButtons[i].Text = "---";
                continue;
            }

            _itemSlotButtons[i].Text = inventory.inventoryItems[i].itemName;
            _inventorySlots.Add(_itemSlotButtons[i], inventory.inventoryItems[i]);
        }
    }

    /// <summary>
    /// going to put this here for now, but will likely just move it to the
    /// decline buttion
    /// </summary>
    public void OnCancelButtonPress()
    {
        _gameBoard.ResetMenu();

        HideMenu();
    }
}
