using Godot;
using System;

namespace lorum;
[GlobalClass]

public partial class Pass : TextureRect
{

	public void moveTo(Vector2 pos)
	{
		Lorum.passIcon.SetPosition(pos);
		this.Visible = true;
		animate();
		//this.Visible = false;
	}
	private void animate()
	{
		this.PivotOffset = this.Size * 0.5f;
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.InOut);
		for (int i = 0; i < 2; i++)
		{
			tween.TweenProperty(this, "scale", new Vector2(1.5f, 1.5f), 0.3f);
			tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.3f);
		}
		tween.Finished += () => this.Visible = false;
	}
}
