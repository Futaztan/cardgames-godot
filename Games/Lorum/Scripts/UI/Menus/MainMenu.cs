using Godot;

namespace cardgames.Games.Lorum.Scripts.UI.Menus;
public partial class MainMenu : Control
{
	[Export(PropertyHint.File, "*.tscn")] 
	private string LorumScenePath  { get; set; }
	[Export(PropertyHint.File, "*.tscn")] 
	private string ZsirScenePath { get; set; }
	[Export(PropertyHint.File, "*.tscn")] 
	private string SettingsScenePath { get; set; }
	
	public void onLorumButtonPressed()
	{
		//GetTree().ChangeSceneToFile("res://Scenes/lorum.tscn");
		//var menuResource = (PackedScene)ResourceLoader.Load("res://Lorum/Scenes/Menus/GameMenu.tscn");
		//var menuInstance = menuResource.Instantiate<cardgames.Lorum.Scripts.UI.Menus.GameMenu>();
		//GetTree().Root.AddChild(menuInstance);
			
		PackedScene scene = GD.Load<PackedScene>(LorumScenePath);
		GetTree().ChangeSceneToPacked(scene);
		//menuInstance.init(GameTypeEnum.LORUM);
	
		///QueueFree();
			
	}
	
	public void onZsirButtonPressed()
	{
	
		PackedScene scene = GD.Load<PackedScene>(ZsirScenePath);
		GetTree().ChangeSceneToPacked(scene);
		//menuInstance.init(GameTypeEnum.LORUM);
		
		//QueueFree();
	}

	private void OnSettingsButtonPressed()
	{
		PackedScene scene = GD.Load<PackedScene>(SettingsScenePath);
		GetTree().ChangeSceneToPacked(scene);
	}
}
