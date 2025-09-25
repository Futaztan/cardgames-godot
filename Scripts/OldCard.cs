using Godot;
using System;
/*
public partial class OldCard : Node2D
{
	private int value;
	private Sprite2D _sprite;
	private Texture2D _altTexture;
		[Signal]
	public delegate void CardClickedEventHandler(OldCard card);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("CardImage");
		//_altTexture = GD.Load<Texture2D>("res://Assets/Cards/makk_7.png");


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetTexture(int val, string path)
	{
		value = val;
		_sprite.Texture = GD.Load<Texture2D>(path);
	}
	public Texture2D getTexture()
	{
		return _sprite.Texture;
	}
	public int getValue() { return value; }
	private void onPlayerCardClicked(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			EmitSignal(nameof(SignalName.CardClicked), this);
		}


	}
}
*/
