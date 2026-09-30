using System;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Cards;
public abstract partial class CardBase : Control
{
	[Export] public float FlipDuration { get; set; } = 0.5f;   // Teljes fordulási idő
	[Export] public float MoveDuration { get; set; } = 0.5f;   // Mozgás ideje
	private int _value = -1;
	protected TextureRect _frontFace;

	public void SetData(int value, Texture2D text)
	{
		_value = value;
		_frontFace.Texture = text;
	}
	internal int GetValue() { return _value; }
	internal Texture2D GetTexture() { return _frontFace.Texture; }

	internal void DeleteCard() { this.QueueFree(); }
	
	

	public abstract Tween Animate(string name, Cell targetCel);
}
