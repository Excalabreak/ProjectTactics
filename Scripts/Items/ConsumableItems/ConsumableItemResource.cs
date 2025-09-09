using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/09/2025]
 * [Resource for Consumable Items]
 */

[GlobalClass]
public partial class ConsumableItemResource : Resource, IInventoryItem, IUseable
{
    [Export] private string _devName;
    [Export] private string _itemName;
    [Export(PropertyHint.MultilineText)] private string _description;

    [Export] private int _size = 1;

    [Export] private bool _infiniteUses;
    [Export] private int _maxUses;
    private int _currentUses;

    [Export] private BaseConsumableEffect _consumableEffect;

    /// <summary>
    /// constructor to initialize the uses
    /// </summary>
    public ConsumableItemResource()
    {
        if (_infiniteUses)
        {
            return;
        }

        _currentUses = _maxUses;
    }

    /// <summary>
    /// on use, do effect
    /// 
    /// if not infinate uses, 
    /// drop current uses
    /// </summary>
    /// <param name="unit"></param>
    public void OnUse(Unit unit)
    {
        _consumableEffect.OnUse(unit);

        if (_infiniteUses)
        {
            return;
        }

        _currentUses--;
    }

    public bool HasUses()
    {
        return _currentUses > 0;
    }

    public string devName
    {
        get { return _devName; }
    }

    public string itemName
    {
        get { return _itemName; }
    }

    public string description
    {
        get { return _description; }
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
