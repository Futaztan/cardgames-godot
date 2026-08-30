using System.Threading.Tasks;
using Godot;

namespace cardgames.Lorum.Scripts.UI;
[GlobalClass]

public partial class Pass : TextureRect
{

	public async Task MoveTo(Vector2 pos)
	{
		this.SetPosition(pos);
		this.Visible = true;
		Tween tween = Animate();
		await this.ToSignal(tween, Tween.SignalName.Finished);
		//this.Visible = false;
	}
	private Tween Animate()
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
		//tween.TweenInterval(0.1f);
		tween.Finished += () => this.Visible = false;
		return tween;
	}
}
