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

    private Button _currentSelectedButton;

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

    /// <summary>
    /// toggles which item is selected
    /// </summary>
    /// <param name="toggle">what state is the button toggled to</param>
    /// <param name="button">node path to button</param>
    private void ToggleItem(bool toggle, NodePath button)
    {
        if (!toggle)
        {
            _currentSelectedButton = null;
            return;
        }

        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.ButtonPressed = false;
        }

        GD.Print(GetNode(button).Name);

        Button toggledButton = GetNode(button) as Button;
        _currentSelectedButton = toggledButton;
        //check to trade item
    }
}
