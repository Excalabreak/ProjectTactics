using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/03/2025]
 * [script for the menu]
 */

public partial class ItemMenu : BaseMenu
{
    [Export] private Button _equippedWeaponButton;
    [Export] private Button[] _itemSlotButtons;

    /// <summary>
    /// calls to set the labels for buttons
    /// </summary>
    /// <param name="inventory">inventory for unit</param>
    public void UpdateButtonText(UnitInventory inventory)
    {
        _equippedWeaponButton.Text = "E: " + inventory.equiptWeapon.itemName;

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                _itemSlotButtons[i].Text = "---";
                continue;
            }

            _itemSlotButtons[i].Text = inventory.inventoryItems[i].itemName;
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
