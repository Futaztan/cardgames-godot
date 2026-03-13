using Godot;
using System;

public partial class GameMenu : Control
{
	//private Lorum gameInstance;
	//private RichTextLabel titleLabel;
	private PopupPanel popupPanel;
	private Settings settings = new Settings(GameTypeEnum.LORUM);

	public override void _Ready()
	{
		base._Ready();
		//titleLabel = GetNode<RichTextLabel>("TitleLabel");
		popupPanel = GetNode<PopupPanel>("PopupPanel");

		popupPanel.Position = (Vector2I)(GetViewportRect().Size - popupPanel.Size) / 2;
		GetNode<Button>("PopupPanel/VBoxContainer/OkButton").Pressed += saveSettings;
		loadSettings();

	}

	public void loadSettings()
	{
		LorumSettings ls = (LorumSettings)settings.loadSettings();
		GetNode<SpinBox>("PopupPanel/VBoxContainer/SpinBox").Value = ls.score;
		GetNode<OptionButton>("PopupPanel/VBoxContainer/OptionButton").Selected = ls.optionIndex;
		GD.Print("SIKERES LOAD");

	}
	/*public void init(GameTypeEnum type)
	{


		PackedScene gameScene;


		switch (type)
		{
			case GameTypeEnum.LORUM:




				break;
			default:
				throw new Exception("enum hiba");


		}


	}*/

	private void saveSettings()
	{


		int score = (int)GetNode<SpinBox>("PopupPanel/VBoxContainer/SpinBox").Value;
		int index = GetNode<OptionButton>("PopupPanel/VBoxContainer/OptionButton").Selected;
		LorumSettings lorumSettings = new LorumSettings(score, index);

		settings.saveSettings(lorumSettings);
		GD.Print("SIKERES MENTES");

	}

	public void OnStartButtonPressed()
	{
		PackedScene gameScene = (PackedScene)ResourceLoader.Load("res://Lorum/Scenes/Menus/Lorum.tscn");
		Lorum gameInstance = gameScene.Instantiate<Lorum>();
		int score = (int)GetNode<SpinBox>("PopupPanel/VBoxContainer/SpinBox").Value;
		int index = GetNode<OptionButton>("PopupPanel/VBoxContainer/OptionButton").Selected;
		gameInstance.init(score, index);
		GetTree().Root.AddChild(gameInstance);
		// QueueFree();
	}
	public void OnExitButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/MainMenu.tscn");
    }

	public void OnOptionButtonPressed()
	{
		popupPanel.Popup();
	}
}
