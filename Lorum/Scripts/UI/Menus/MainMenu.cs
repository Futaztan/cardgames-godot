using Godot;
using System;

namespace lorum;
public partial class MainMenu : Control
{
	public void onLorumButtonPressed()
	{
		//GetTree().ChangeSceneToFile("res://Scenes/lorum.tscn");
		var menuResource = (PackedScene)ResourceLoader.Load("res://Lorum/Scenes/Menus/GameMenu.tscn");
		var menuInstance = menuResource.Instantiate<GameMenu>();
		//GetTree().Root.AddChild(menuInstance);
		GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");
		//menuInstance.init(GameTypeEnum.LORUM);
	
		///QueueFree();
			
	}
	
	public void onZsirButtonPressed()
	{
	
	
		GetTree().ChangeSceneToFile("res://Zsirozas/Zsir.tscn");
		//menuInstance.init(GameTypeEnum.LORUM);
		
		//QueueFree();
	}
}
