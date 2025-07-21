using Godot;
using System;

public partial class PrototypeEndMenu : CanvasLayer
{
    [Export] private Label _titleLable;

    [Export] private GridCursor _gridCursor;
    [Export] private Button _cursorDefaultStartButton;

    public override void _Ready()
    {
        UnitEventManager.UnitDeathEvent += IsCommanderDead;
    }

    public override void _ExitTree()
    {
        UnitEventManager.UnitDeathEvent -= IsCommanderDead;
    }

    public void IsCommanderDead(Unit unit)
    {
        if (!unit.isCommander)
        {
            return;
        }

        OnEndScreen(unit.unitGroup != UnitGroupEnum.PLAYER);
    }

    public void OnEndScreen(bool win)
    {
        if (win)
        {
            _titleLable.Text = "You Won";
        }
        else
        {
            _titleLable.Text = "You Lose";
        }

        _cursorDefaultStartButton.GrabFocus();


        this.Visible = true;
        _gridCursor.Hide();
        _gridCursor.ProcessMode = ProcessModeEnum.Disabled;
    }

    public void OnQuit()
    {
        OS.ShellOpen("https://docs.google.com/forms/d/e/1FAIpQLSe5u3DWhBJQbsupk4QQpsxfhtFOVgcuVx0ZrK49ntYTb9w5Zg/viewform?usp=dialog");
        GetTree().Quit();
    }
}
