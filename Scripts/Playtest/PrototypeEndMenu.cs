using Godot;
using System;

public partial class PrototypeEndMenu : CanvasLayer
{
    [Export] private Label _titleLable;

    [Export] private GridCursor _gridCursor;
    [Export] private Button _cursorDefaultStartButton;

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
        OS.ShellOpen("https://youtu.be/dQw4w9WgXcQ?si=eOyFSowUIdmKRR_1");
        GetTree().Quit();
    }
}
