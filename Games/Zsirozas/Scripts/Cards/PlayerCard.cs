
using Godot;
using System;

namespace cardgames.Games.Zsirozas.Scripts.Cards;
public partial class PlayerCard : CardBase
{

	[Signal]
	public delegate void CardClickedEventHandler(PlayerCard card);

	public override void _Ready()
	{
		_frontFace = GetNode<TextureRect>("FrontFace");
	}

	public void DisableCard(bool setToGrey = true)
	{
		MouseFilter = MouseFilterEnum.Ignore;
		if (setToGrey)
		{
			_frontFace.Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
		}
	}

	public void EnableCard()
	{
		MouseFilter = MouseFilterEnum.Pass;
		_frontFace.Modulate = new Color(1f, 1f, 1f, 1f); 
	}


	public override Tween Animate(string name, Cell targetCell)
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
		return tween;
	}
	public void onPlayerCardClicked(InputEvent @event)
	{
		Vector2 _pressedPos = new Vector2(0,0);
		bool _isDragging = false;
		float _dragThreshold = 10f;

		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseButtonEvent.Pressed)
			{
				_pressedPos = mouseButtonEvent.Position;
				_isDragging = false;
			}
			else
			{
				if (!_isDragging)
				{
					// Csak akkor számít kattintásnak, ha nem volt drag
					EmitSignal(SignalName.CardClicked,this);
				}
			}
		}
		else if (@event is InputEventMouseMotion mouseMotionEvent && mouseMotionEvent.ButtonMask.HasFlag(MouseButtonMask.Left))
		{
			if (mouseMotionEvent.Position.DistanceTo(_pressedPos) > _dragThreshold)
			{
				_isDragging = true;
			}
		}
	}
	
}
