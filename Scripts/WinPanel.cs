using Godot;
using System;

public partial class WinPanel : Control
{
	private void _on_continue_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}

	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}
}