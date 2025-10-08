using Godot;
using System;

public partial class GameMenu : Control
{
    private Lorum gameInstance;
    public void init(GameTypeEnum type) //TODO enum vagy int
    {
        PackedScene gameScene;

        switch (type)
        {
            case GameTypeEnum.LORUM:
                gameScene = (PackedScene)ResourceLoader.Load("res://Scenes/Lorum.tscn");
                gameInstance = gameScene.Instantiate<Lorum>();
                gameInstance.init(10, -1);

                break;
            default:
                gameScene = (PackedScene)ResourceLoader.Load("res://Scenes/Lorum.tscn");
                gameInstance = gameScene.Instantiate<Lorum>();
                gameInstance.init(10, -1);
                break;

        }

    }

    public void OnStartButtonPressed()
    {
        GetTree().Root.AddChild(gameInstance);
    }
}
