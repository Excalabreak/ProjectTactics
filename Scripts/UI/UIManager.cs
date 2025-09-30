using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/21/2025]
 * [Manages UI Elements and 
 * is one class to call them all from]
 */

public partial class UIManager : CanvasLayer
{
    [Export] private UIStats _uiStats;
    [Export] private UIBattle _uiBattle;
    [Export] private UITerrain _uiTerrain;
    [Export] private UIInventory _uiInventroy;
    [Export] private UIBattleLog _uiBattleLog;
    [Export] private Panel _controlsPanel;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ToggleControls"))
        {
            _controlsPanel.Visible = !_controlsPanel.Visible;
            GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>
    /// calls to displays the stats of a unit
    /// </summary>
    /// <param name="unit">unit being displayed</param>
    public void ShowUnitStats(Unit unit)
    {
        _uiStats.ShowUnitStats(unit);
    }

    /// <summary>
    /// calls to hides the stats pannel
    /// </summary>
    public void HideStatsPanel()
    {
        _uiStats.HideStatsPanel();
    }

    /// <summary>
    /// calls to displays the inventory of a unit
    /// </summary>
    /// <param name="unit">unit being displayed</param>
    public void ShowUnitInventory(Unit unit)
    {
        _uiInventroy.ShowInventory(unit.unitInventory, unit.unitName);
    }

    /// <summary>
    /// calls to hides the inventory pannel
    /// </summary>
    public void HideUnitInventory()
    {
        _uiInventroy.HideInventory();
    }

    /// <summary>
    /// shows terrain info
    /// </summary>
    /// <param name="name">name of terrain</param>
    /// <param name="move">move cost of terrain</param>
    /// <param name="vision">vision cost of terrain</param>
    public void ShowTerrainPanel(string name, float move, float vision)
    {
        _uiTerrain.ShowTerrainPanel(name, move, vision);
    }

    /// <summary>
    /// hides the stats pannel
    /// </summary>
    public void HideTerrainPanel()
    {
        _uiTerrain.HideTerrainPanel();
    }

    /// <summary>
    /// shows the battle prediction for the battle
    /// </summary>
    /// <param name="playerHP">player hp</param>
    /// <param name="enemyHP">enemy hp</param>
    /// <param name="playerDMG">damage player deals</param>
    /// <param name="enemyDMG">damage enemy deals</param>
    /// <param name="playerACC">player accurecy</param>
    /// <param name="enemyACC">enemy accurecy</param>
    /// <param name="playerCRT">player crit chance</param>
    /// <param name="enemyCRT">enemy crit chance</param>
    public void ShowBattlePredictions(int playerHP, int enemyHP, int playerDMG, int enemyDMG,
        int playerACC, int enemyACC, int playerCRT, int enemyCRT)
    {
        _uiBattle.ShowBattlePredictions(playerHP, enemyHP, playerDMG, enemyDMG,
        playerACC, enemyACC, playerCRT, enemyCRT);
    }

    /// <summary>
    /// hides battle predictions
    /// </summary>
    public void HideBattlePredictions()
    {
        _uiBattle.HideBattlePredictions();
    }

    /// <summary>
    /// calls to add to the battle log
    /// </summary>
    /// <param name="message"></param>
    public void AddToBattleLog(string message)
    {
        _uiBattleLog.AddToBattleLog(message);
    }
}