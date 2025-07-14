using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/14/2025]
 * [displays battle predictions]
 */

public partial class UIBattle : Control
{
    [Export] private Label _playerHPLable;
    [Export] private Label _playerDMGLable;
    [Export] private Label _playerACCLable;
    [Export] private Label _playerCRTLable;

    [Export] private Label _enemyHPLable;
    [Export] private Label _enemyDMGLable;
    [Export] private Label _enemyACCLable;
    [Export] private Label _enemyCRTLable;


    public void ShowBattlePredictions(int playerHP, int enemyHP, int playerDMG, int enemyDMG, 
        int playerACC, int enemyACC, int playerCRT, int enemyCRT)
    {
        _playerHPLable.Text = "" + playerHP + " -> " + (playerHP - enemyDMG);
        _playerDMGLable.Text = "" + playerDMG;
        _playerACCLable.Text = "" + playerACC;
        _playerCRTLable.Text = "" + playerCRT;

        _enemyHPLable.Text = "" + enemyHP + " -> " + (enemyHP - playerDMG);
        _enemyDMGLable.Text = "" + enemyDMG;
        _enemyACCLable.Text = "" + enemyACC;
        _enemyCRTLable.Text = "" + enemyCRT;

        this.Visible = true;
    }

    public void HideBattlePredictions()
    {
        this.Visible = false;
    }
}
