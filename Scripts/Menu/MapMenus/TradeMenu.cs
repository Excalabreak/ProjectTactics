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
        //might need to take this out for updating ui
        _leftUnitMenu.SetUpGroup(firstUnit);
        _rightUnitMenu.SetUpGroup(secondUnit);

        _leftInventroy = firstUnit.unitInventory;
        _rightInventroy = secondUnit.unitInventory;
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
    /// </summary>
    public void AttemptTrade()
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

        //NOTE: only have weapon for now, will likely get more complicated with armor and accessories
        bool leftGoToInventory = _leftUnitMenu.currentlySelectedButton != _leftUnitMenu.equippedWeaponButton;
        bool rightGoToInventory = _rightUnitMenu.currentlySelectedButton != _rightUnitMenu.equippedWeaponButton;

        //possible for item to be equipment
        //possible for item to be null



        //if both are equipped weapon just swap them. might need to change later if i want to include offhand
        //if one is equipped and one is inventory, then check if equipt can swap with inventory slot
        //if both are inventory, both has to check before swap

        //check if it's equipt weapon since that doesn't require any inventory slots
        //check if items can be replaced in inventory

        //_currentSelectedButton.ButtonPressed = false;
        //update trade ui
    }

    
    private bool CheckOneSideForTrade(Button newItemButton, Button oldItemButton, TradeMenuGroup checkGroup, UnitInventory checkInventory)
    {
        IInventoryItem newItem;
        IInventoryItem oldItem;

        if (_itemButtons.ContainsKey(newItemButton))
        {
            newItem = _itemButtons[newItemButton];
        }
        else
        {
            //because nothing can go into an inventory
            return true;
        }
        if (_itemButtons.ContainsKey(oldItemButton))
        {
            oldItem = _itemButtons[oldItemButton];
        }

        IEquipable equipableCheck = newItem as IEquipable;
        EquipableSlotEnum equipableSlot;

        if (equipableCheck != null)
        {
            equipableSlot = equipableCheck.equipableSlot;
        }

        if (oldItemButton == checkGroup.equippedWeaponButton)
        {
            //if new item is not a weapon
            //skip to attempt to store it in the inventory
            //else
            //return true (tho future will take in account for useable weapons)
        }

        //inventory check
        return true;
    }
}
