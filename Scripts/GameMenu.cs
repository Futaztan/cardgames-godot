using Godot;
using System;

public partial class GameMenu : Control
{
    private Lorum gameInstance;
    private RichTextLabel titleLabel;
    private PopupPanel popupPanel;
    private Settings settings = new Settings(GameTypeEnum.LORUM);

    public override void _Ready()
    {
        base._Ready();
        titleLabel = GetNode<RichTextLabel>("TitleLabel");
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
    public void init(GameTypeEnum type)
    {


        PackedScene gameScene;
        string title;

        switch (type)
        {
            case GameTypeEnum.LORUM:
                title = "Lórum";
                gameScene = (PackedScene)ResourceLoader.Load("res://Scenes/Lorum.tscn");
                gameInstance = gameScene.Instantiate<Lorum>();
               

                break;
            default:
                throw new Exception("enum hiba");


        }
        titleLabel.Text = title;

    }

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
         int score = (int)GetNode<SpinBox>("PopupPanel/VBoxContainer/SpinBox").Value;
        int index = GetNode<OptionButton>("PopupPanel/VBoxContainer/OptionButton").Selected;
        gameInstance.init(score, index);
        GetTree().Root.AddChild(gameInstance);
    }

    public void OnOptionButtonPressed()
    {
        popupPanel.Popup();
    }
}
