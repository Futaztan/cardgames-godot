using System;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Cards;
public partial class BackCard : CardBase
{

	private TextureRect _backFace;

	private bool showingFront = false;

	public override void _Ready()
	{
		_backFace = GetNode<TextureRect>("BackFace");
		_frontFace = GetNode<TextureRect>("FrontFace");
		this.PivotOffset = this.Size * 0.5f;
		Scale = Vector2.One;
	}

	public override Tween Animate(string name, Cell targetCel)
	{


		Vector2 targetGlobalPos = targetCel.GlobalPosition;
		Vector2 targetScale = targetCel.Size / this._frontFace.Size;

		float half = FlipDuration * 0.5f;
		PivotOffset = Size * 0.5f;
		if (name != "bot2")
		{
			TextureRect backFaceRotated = GetNode<TextureRect>("BackFaceRotated");
			backFaceRotated.Visible = false;
			_backFace.Visible = true;
		}
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.InOut);

		tween.TweenProperty(this, "scale:x", 0f, half);
		tween.TweenCallback(Callable.From(() => SwapFace()));
		tween.TweenProperty(this, "global_position", targetGlobalPos, MoveDuration);
		tween.Parallel().TweenProperty(this, "scale:x", targetScale.X, half);
		tween.Parallel().TweenProperty(this, "scale:y", targetScale.Y, half);
		return tween;

	}

	private void SwapFace()
	{
		_backFace.Visible = false;
		_frontFace.Visible = true;
	}



	/* public override void _GuiInput(InputEvent @event)
	 {
		 if (@event is InputEventMouseButton mouseEvent &&
			 mouseEvent.Pressed &&
			 mouseEvent.ButtonIndex == MouseButton.Left)
		 {
			 // Példa: bal kattintásra mozduljon át ide és közben forduljon
			 Vector2 target = new Vector2(600, 200);
			 FlipAndMove(target);
		 }
	 }*/


}
