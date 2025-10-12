using Godot;
using System;

public partial class MainMenu : Control
{
	public void onLorumButtonPressed()
	{
		//GetTree().ChangeSceneToFile("res://Scenes/lorum.tscn");
		var menuResource = (PackedScene)ResourceLoader.Load("res://Scenes/GameMenu.tscn");
		var menuInstance = menuResource.Instantiate<GameMenu>();
		GetTree().Root.AddChild(menuInstance);
		menuInstance.init(GameTypeEnum.LORUM);
		//this.Hide();
			
	}
}
