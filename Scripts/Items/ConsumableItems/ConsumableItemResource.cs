using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
 * [Resource for Consumable Items]
 */

[GlobalClass]
public partial class ConsumableItemResource : Resource, IInventoryItem, IUseable
{
    [Export] private string _itemName;
    [Export] private int _size = 1;

    [Export] private bool _infiniteUses;
    [Export] private int _maxUses;

    [Export] private BaseConsumableEffect _consumableEffect;

    public void OnUse(Unit unit)
    {
        _consumableEffect.OnUse(unit);
    }

    public string itemName
    {
        get { return _itemName; }
    }

    public int size
    {
        get { return _size; }
    }

    public bool infiniteUses
    {
        get { return _infiniteUses; }
    }

    public int maxUses
    {
        get { return _maxUses; }
    }
}
