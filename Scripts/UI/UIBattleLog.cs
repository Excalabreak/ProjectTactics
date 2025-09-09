using Godot;
using System;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/09/2025]
 * [ui pannel for what happend in the battle]
 */

public partial class UIBattleLog : Control
{
    [Export] private ScrollContainer _scrollContainer;
    [Export] private Control _labelLocation;

    private VScrollBar _scrollBar;

    [Export] private int _historyLength = 20;
    private List<Label> _messageHistory = new List<Label>();

    public override void _Ready()
    {
        UnitEventManager.UnitDeathEvent += OnUnitDeath;

        _scrollBar = _scrollContainer.GetVScrollBar();
        _scrollBar.Changed += AutoScroll;
    }

    public override void _ExitTree()
    {
        UnitEventManager.UnitDeathEvent -= OnUnitDeath;
        _scrollBar.Changed -= AutoScroll;
    }

    /// <summary>
    /// creates a lable and adds it to the battle log
    /// </summary>
    /// <param name="message">what gets displayed</param>
    public void AddToBattleLog(string message)
    {
        if (_messageHistory.Count >= _historyLength)
        {
            _messageHistory[0].QueueFree();
            _messageHistory.RemoveAt(0);
        }

        Label messageLabel = new Label();
        messageLabel.Text = message;
        _messageHistory.Add(messageLabel);

        _labelLocation.AddChild(messageLabel);
        _scrollContainer.ScrollVertical = 9999999;
    }

    /// <summary>
    /// on a unit death, 
    /// say the unit died in battle log
    /// </summary>
    /// <param name="unit"></param>
    private void OnUnitDeath(Unit unit)
    {
        AddToBattleLog(unit.Name + " died.");
    }

    /// <summary>
    /// scrolls the chat to the bottom if needed
    /// </summary>
    private void AutoScroll()
    {
        _scrollContainer.ScrollVertical = (int)_scrollBar.MaxValue;
    }
}
