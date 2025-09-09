using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/29/2025]
 * [group for trade menu for one unit]
 */

public partial class TradeMenuGroup : Node
{
    [Export] private TradeMenu _tradeMenu;

    [Export] private Button _equippedWeaponButton;
    [Export] private Button[] _itemSlotButtons;

    [Export] private bool _isLeftGroup = false;

    private Button _currentlySelectedButton;

    /// <summary>
    /// calls to update button lable
    /// also adds all buttons to dictionary
    /// </summary>
    /// <param name="unit"></param>
    public void SetUpGroup(Unit unit)
    {
        UnitInventory inventory = unit.unitInventory;

        UpdateButtonText(inventory);

        _tradeMenu.AddToButtonDictionary(_equippedWeaponButton, inventory.equiptWeapon);

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                break;
            }
            _tradeMenu.AddToButtonDictionary(_itemSlotButtons[i], inventory.inventoryItems[i]);
        }
    }

    /// <summary>
    /// resets the buttons
    /// </summary>
    /// <param name="inventory">inventory for group</param>
    public void ResetGroup(UnitInventory inventory)
    {
        _currentlySelectedButton.ButtonPressed = false;
        _currentlySelectedButton = null;

        UpdateButtonText(inventory);

        _tradeMenu.AddToButtonDictionary(_equippedWeaponButton, inventory.equiptWeapon);

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                break;
            }
            _tradeMenu.AddToButtonDictionary(_itemSlotButtons[i], inventory.inventoryItems[i]);
        }
    }

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
    /// toggles which item is selected
    /// </summary>
    /// <param name="toggle">what state is the button toggled to</param>
    /// <param name="button">node path to button</param>
    private void ToggleItem(bool toggle, NodePath button)
    {
        if (!toggle)
        {
            _currentlySelectedButton = null;
            _tradeMenu.ReEnableAllButtons(!_isLeftGroup);
            return;
        }

        _currentlySelectedButton = GetNode(button) as Button;

        if (_currentlySelectedButton == null)
        {
            GD.Print("trade menu toggle item doesn't have correct node path");
        }

        if (!_tradeMenu.CanTrade())
        {
            _tradeMenu.DisableOppositeButtons(_currentlySelectedButton, _isLeftGroup);
        }
        else
        {
            _tradeMenu.TradeSelectedItems();
        }
    }

    public Button currentlySelectedButton
    {
        get { return _currentlySelectedButton; }
    }

    public Button equippedWeaponButton
    {
        get { return _equippedWeaponButton; }
    }

    public Button[] itemSlotButtons
    {
        get { return _itemSlotButtons; }
    }
}
