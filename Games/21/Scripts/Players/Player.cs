using Godot;

namespace cardgames.Games._21.Scripts.Players;

public class Player : EntityBase
{
    public Player(string name, int id, CardContainer container, int money, Label label) : base(name, id, container, money, label)
    {
    }
}