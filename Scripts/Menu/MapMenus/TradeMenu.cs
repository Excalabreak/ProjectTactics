using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [08/27/2025]
 * [menu for trading]
 */

public partial class TradeMenu : BaseMenu
{
    [Export] private TradeMenuGroup _leftUnitMenu;
    [Export] private TradeMenuGroup _rightUnitMenu;

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
        _leftUnitMenu.SetUpGroup(firstUnit);
        _rightUnitMenu.SetUpGroup(secondUnit);
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
}
