using Godot;

namespace cardgames.Games._21.Scripts.Cards;

public abstract partial class CardBase : Control
{
    protected float FlipDuration = 0.5f; // Teljes fordulási idő
    protected float MoveDuration = 0.5f; // Mozgás ideje
    protected int _value = -1;
    protected TextureRect _frontFace;

    public void setDatas(int value, Texture2D text)
    {
        _value = value;
        _frontFace.Texture = text;
    }

    internal int getValue()
    {
        return _value;
    }

    internal Texture2D getTexture()
    {
        return _frontFace.Texture;
    }

    internal void deleteCard()
    {
        this.QueueFree();
    }


    public abstract Tween Animate(string name, Cell targetCell);
}