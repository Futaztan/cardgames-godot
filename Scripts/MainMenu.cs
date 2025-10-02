using Godot;
using System;

public partial class MainMenu : Control
{
	public void onLorumButtonPressed()
	{
		//GetTree().ChangeSceneToFile("res://Scenes/lorum.tscn");
		var menuResource = (PackedScene)ResourceLoader.Load("res://Scenes/GameMenutscn");
		var menuInstance = menuResource.Instantiate<GameMenu>();
		menuInstance.init();
		GetTree().Root.AddChild(menuInstance);
		//this.Hide();
			
	}
}
