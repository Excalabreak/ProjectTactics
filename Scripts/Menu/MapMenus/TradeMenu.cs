using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/29/2025]
 * [menu for trading]
 */

public partial class TradeMenu : BaseMenu
{
    [Export] private TradeMenuGroup _leftUnitMenu;
    [Export] private TradeMenuGroup _rightUnitMenu;

    private UnitInventory _leftInventroy;
    private UnitInventory _rightInventroy;
    private Dictionary<Button, IInventoryItem> _itemButtons = new Dictionary<Button, IInventoryItem>();

    /// <summary>
    /// sets up the buttons for trade menu
    /// definatly a better way of doing this
    /// 
    /// gets the whole unit for name
    /// </summary>
    /// <param name="firstUnit">unit one</param>
    /// <param name="secondUnit">unit two</param>
    public void SetUpTradeMenu(Unit firstUnit, Unit secondUnit)
    {
        _itemButtons = new Dictionary<Button, IInventoryItem>();

        //might need to take this out for updating ui
        _leftUnitMenu.SetUpGroup(firstUnit);
        _rightUnitMenu.SetUpGroup(secondUnit);

        _leftInventroy = firstUnit.unitInventory;
        _rightInventroy = secondUnit.unitInventory;
    }

    /// <summary>
    /// resets the trade menu
    /// </summary>
    private void ResetTradeMenu()
    {
        _itemButtons = new Dictionary<Button, IInventoryItem>();

        _leftUnitMenu.ResetGroup(_leftInventroy);
        _rightUnitMenu.ResetGroup(_rightInventroy);
    }

    /// <summary>
    /// gives TradeMenuGroup a way of adding buttons to dictionaries
    /// </summary>
    /// <param name="key">button item is tied to</param>
    /// <param name="value">item button is tied to</param>
    public void AddToButtonDictionary(Button key, IInventoryItem value)
    {
        if (_itemButtons.ContainsKey(key))
        {
            _itemButtons.Remove(key);
        }

        _itemButtons.Add(key, value);
    }

    /// <summary>
    /// disables all buttons that the selected button cannot trade with
    /// </summary>
    /// <param name="selectedItemButton">which item is selected</param>
    /// <param name="isLeftGroup">which group is item FROM</param>
    public void DisableOppositeButtons(Button selectedItemButton, bool isLeftGroup)
    {
        ReEnableAllButtons(!isLeftGroup);
        IInventoryItem selectedItem;

        if (!_itemButtons.ContainsKey(selectedItemButton))
        {
            return;
        }

        selectedItem = _itemButtons[selectedItemButton];

        TradeMenuGroup toGroup = _leftUnitMenu;
        UnitInventory toInventory = _leftInventroy;

        if (isLeftGroup)
        {
            toGroup = _rightUnitMenu;
            toInventory = _rightInventroy;
        }

        IEquipable equipableCheck = selectedItem as IEquipable;
        EquipableSlotEnum equipableSlot;

        if (equipableCheck == null)
        {
            toGroup.equippedWeaponButton.Disabled = true;
            //toGroup.equippedArmorButton.Disabled = true;
            //toGroup.equippedAccessoriesButton.Disabled = true;
        }
        else
        {
            equipableSlot = equipableCheck.equipableSlot;
            switch (equipableSlot)
            {
                case EquipableSlotEnum.WEAPON:
                    //toGroup.equippedArmorButton.Disabled = true;
                    //toGroup.equippedAccessoriesButton.Disabled = true;
                    break;
                case EquipableSlotEnum.ARMOR:
                    toGroup.equippedWeaponButton.Disabled = true;
                    //toGroup.equippedAccessoriesButton.Disabled = true;
                    break;
                case EquipableSlotEnum.ACCESSORY:
                    toGroup.equippedWeaponButton.Disabled = true;
                    //toGroup.equippedArmorButton.Disabled = true;
                    break;
                default:
                    toGroup.equippedWeaponButton.Disabled = true;
                    //toGroup.equippedArmorButton.Disabled = true;
                    //toGroup.equippedAccessoriesButton.Disabled = true;
                    break;
            }
        }

        bool disable = false;
        //loop through all item slots and see what can be traded
        foreach (Button itemSlotButton in toGroup.itemSlotButtons)
        {
            disable = false;
            if (!_itemButtons.ContainsKey(itemSlotButton))
            {
                if (!toInventory.CanAddItemToInventory(selectedItem))
                {
                    disable = true;
                }
            }
            else
            {
                if (!toInventory.CanReplaceInventoryItem(selectedItem, _itemButtons[itemSlotButton]))
                {
                    disable = true;
                }
            }

            itemSlotButton.Disabled = disable;
        }
    }

    /// <summary>
    /// re enables all buttons
    /// </summary>
    /// <param name="group">which side to re enable buttons</param>
    public void ReEnableAllButtons(bool group)
    {
        TradeMenuGroup enableGroup = _rightUnitMenu;
        if (group)
        {
            enableGroup = _leftUnitMenu;
        }


        enableGroup.equippedWeaponButton.Disabled = false;
        foreach (Button button in enableGroup.itemSlotButtons)
        {
            button.Disabled = false;
        }
    }

    /// <summary>
    /// checks if both have selected an item to trade
    /// </summary>
    /// <returns>true if a trade can be attempted</returns>
    public bool CanTrade()
    {
        return _leftUnitMenu.currentlySelectedButton != null && _rightUnitMenu.currentlySelectedButton != null;
    }

    /// <summary>
    /// checks if an item is selected on both sides
    /// and trades the items if true
    /// 
    /// NOTE: ASSUMES THE SELECTED OPTIONS ARE ABLE TO TRADE
    /// </summary>
    public void TradeSelectedItems()
    {
        if (_leftUnitMenu.currentlySelectedButton == null || _rightUnitMenu.currentlySelectedButton == null)
        {
            return;
        }

        IInventoryItem leftItem;
        IInventoryItem rightItem;

        if (_itemButtons.ContainsKey(_leftUnitMenu.currentlySelectedButton))
        {
            leftItem = _itemButtons[_leftUnitMenu.currentlySelectedButton];
        }
        if (_itemButtons.ContainsKey(_rightUnitMenu.currentlySelectedButton))
        {
            rightItem = _itemButtons[_rightUnitMenu.currentlySelectedButton];
        }

        //remove items from old inventory
        RemoveItemFromInventory(_leftUnitMenu, _leftInventroy);
        RemoveItemFromInventory(_rightUnitMenu, _rightInventroy);

        AddItemToInventory(_leftUnitMenu, _rightUnitMenu, _rightInventroy);
        AddItemToInventory(_rightUnitMenu, _leftUnitMenu, _leftInventroy);

        ResetTradeMenu();
    }

    /// <summary>
    /// removes item from inventory group
    /// </summary>
    /// <param name="tradeMenu">menu of inventory</param>
    /// <param name="inventory">inventory of menu group</param>
    private void RemoveItemFromInventory(TradeMenuGroup tradeMenu, UnitInventory inventory)
    {
        if (tradeMenu.currentlySelectedButton == null)
        {
            return;
        }
        if (!_itemButtons.ContainsKey(tradeMenu.currentlySelectedButton))
        {
            return;
        }

        if (tradeMenu.currentlySelectedButton == tradeMenu.equippedWeaponButton)
        {
            inventory.RemoveEquippedWeapon();
        }
        //else if for other equip slots
        else
        {
            inventory.RemoveInventoryItem(_itemButtons[tradeMenu.currentlySelectedButton]);
        }
    }

    /// <summary>
    /// adds item from trade to approprite place
    /// </summary>
    /// <param name="fromMenu">the giver's menu group</param>
    /// <param name="toMenu">the receiver's menu group</param>
    /// <param name="inventory">the receiver's inventory</param>
    private void AddItemToInventory(TradeMenuGroup fromMenu, TradeMenuGroup toMenu, UnitInventory inventory)
    {
        if (fromMenu.currentlySelectedButton == null)
        {
            return;
        }
        if (!_itemButtons.ContainsKey(fromMenu.currentlySelectedButton))
        {
            return;
        }

        if (toMenu.currentlySelectedButton == toMenu.equippedWeaponButton)
        {
            inventory.EquipWeapon(_itemButtons[fromMenu.currentlySelectedButton]);
        }
        //else if for other equip slots
        else
        {
            inventory.AddInventoryItem(_itemButtons[fromMenu.currentlySelectedButton]);
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
