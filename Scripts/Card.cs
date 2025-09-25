using Godot;
using System;


[GlobalClass]
public partial class Card : CardBase
{

	[Signal]
	public delegate void CardClickedEventHandler(Card card);

	public override void _Ready()
	{
		_frontFace = GetNode<TextureRect>("FrontFace");
	}
	internal void disableCard()
	{
		   MouseFilter = MouseFilterEnum.Ignore;
	}
	internal void enableCard()
	{
		   MouseFilter = MouseFilterEnum.Pass;
	}

	
	public override void Animate(string name, Cell targetCell, Action onDone) //BOT2 JÓ
	{

		float half = FlipDuration * 0.5f;

		// Tween létrehozása
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic);

		tween.SetEase(Tween.EaseType.InOut);


		Vector2 targetGlobalPos = targetCell.GlobalPosition;
		tween.TweenProperty(this, "global_position", targetGlobalPos, MoveDuration);

		/*// Első félfordulat: 1 → 0
		tween.TweenProperty(this, "scale:x", 0.0f, half);

		// Félidőnél textúra csere
		//tween.TweenCallback(Callable.From(() => SwapFace(cardText)));

		// Második félfordulat: 0 → 1
		tween.TweenProperty(this, "scale:x", 1.0f, half);*/

		// Ha kell callback a végére:
		//tween.TweenCallback(Callable.From(OnAnimDone));
		tween.Finished += () => onDone();
	}
	


	public void onPlayerCardClicked(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			// Ellenőrizzük, hogy a bal gombot nyomták-e le és hogy ez egy "lenyomott" esemény.
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				GD.Print("A Control node-ra kattintottak!");
				EmitSignal(SignalName.CardClicked,this);
				
			}
		}
	}
}
