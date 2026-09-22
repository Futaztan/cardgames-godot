using Godot;

namespace cardgames.Games._21.Scripts.Players;

public class Player : EntityBase
{
    public Player(string name, int id, CardContainer container, int money, Label label, Label valueLabel, GoldCoin goldCoin) : base(name, id, container, label, valueLabel, goldCoin)
    {
        PackedScene cardScene = GD.Load<PackedScene>("res://Games/21/Scenes/Cards/PlayerCard.tscn");
        Control cardInstance = (Control)cardScene.Instantiate();
        CardSize = cardInstance.Size;
        cardInstance.QueueFree();
    }

   
}