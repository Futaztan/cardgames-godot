using System;
using System.Collections.Generic;
using Godot;

public abstract partial class CardBase : Control
{
    [Export] public float FlipDuration { get; set; } = 0.6f;   // Teljes fordulási idő
    [Export] public float MoveDuration { get; set; } = 0.7f;   // Mozgás ideje
    protected int _value = -1;
    protected TextureRect _frontFace;

    public void setDatas(int value, Texture2D text)
    {
        _value = value;
        _frontFace.Texture = text;
    }
    internal int getValue() { return _value; }
    internal Texture2D getTexture() { return _frontFace.Texture; }

    internal void deleteCard() { this.QueueFree(); }
    
    

    public abstract void Animate( string name,Cell targetCell, Action onDone);
}