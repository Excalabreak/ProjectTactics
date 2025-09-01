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

    private Button _currentSelectedButton;
    private bool _isCurrentLeftGroup = false;

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
    /// checks if an item is selected on both sides
    /// and trades the items if true
    /// </summary>
    public void CheckForTrade(Button itemButton, bool isLeftGroup)
    {
        //maybe have it so if unit is engaged in combat, can't change equipped weapon
        if (_currentSelectedButton == null)
        {
            _currentSelectedButton = itemButton;
            _isCurrentLeftGroup = isLeftGroup;
            return;
        }

        IInventoryItem leftItem;
        IInventoryItem rightItem;

        bool isCurrentEmpty = false;
        bool isNewEmpty = false;

        if (!_itemButtons.ContainsKey(_currentSelectedButton))
        {
            isCurrentEmpty = true;
        }
        if (!_itemButtons.ContainsKey(itemButton))
        {
            isNewEmpty = true;
        }

        if (_isCurrentLeftGroup)
        {
            if (!isCurrentEmpty)
            {
                leftItem = _itemButtons[_currentSelectedButton];
            }
            rightItem = _itemButtons[itemButton];
        }
        else
        {
            leftItem = _itemButtons[itemButton];
            rightItem = _itemButtons[_currentSelectedButton];
        }

        //if both are equipped weapon just swap them. might need to change later if i want to include offhand
        //if one is equipped and one is inventory, then check if equipt can swap with inventory slot
        //if both are inventory, both has to check before swap

        //check if it's equipt weapon since that doesn't require any inventory slots
        //check if items can be replaced in inventory

        //_currentSelectedButton.ButtonPressed = false;
        //update trade ui
    }

    /// <summary>
    /// unselects current item to trade
    /// </summary>
    public void UnselectItem()
    {
        _currentSelectedButton = null;
    }

    public Button currentSelectedButton
    {
        get { return _currentSelectedButton; }
    }
}
