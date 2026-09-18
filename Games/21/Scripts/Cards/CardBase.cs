using Godot;

namespace cardgames.Games._21.Scripts.Cards;

public abstract partial class CardBase : Control
{
    protected float FlipDuration = 0.5f; // Teljes fordulási idő
    protected float MoveDuration = 0.5f; // Mozgás ideje
    public int Value { get; private set; } = -1;
    protected TextureRect FrontFace { get; set; }
    public int ScoreValue { get; private set; }

    public void SetDatas(int value,int score, Texture2D text)
    {
        Value = value;
        ScoreValue = score;
        FrontFace.Texture = text;
    }
    

    public Texture2D GetTexture()
    {
        return FrontFace.Texture;
    }

    public void DeleteCard()
    {
        this.QueueFree();
    }


    public abstract Tween Animate(string name, Cell targetCell);
}