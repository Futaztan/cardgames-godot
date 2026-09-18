using Godot;

namespace cardgames.Games._21.Scripts.Cards;

public partial class BackCard : CardBase
{

    private TextureRect _backFace;

    private bool showingFront = false;

    public override void _Ready()
    {
        _backFace = GetNode<TextureRect>("BackFace");
        FrontFace = GetNode<TextureRect>("FrontFace");
        this.PivotOffset = this.Size * 0.5f;
        Scale = Vector2.One;
    }

    public override Tween Animate(string name, Cell targetCell)
    {


        Vector2 targetGlobalPos = targetCell.GlobalPosition;
        Vector2 targetScale = targetCell.Size / this.FrontFace.Size;

        float half = FlipDuration * 0.5f;

        if (name != "bot2")
        {
            TextureRect backFaceRotated = GetNode<TextureRect>("BackFaceRotated");
            backFaceRotated.Visible = false;
            _backFace.Visible = true;
        }


        // Biztosítjuk, hogy a kártya a saját közepéből forogjon és méreteződjön
        PivotOffset = Size * 0.5f;

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Cubic);
        tween.SetEase(Tween.EaseType.InOut);

        // 1. Kártya becsukása X tengelyen (Flip első fele)
        tween.TweenProperty(this, "scale:x", 0f, half);

        // 2. Kép átváltása a hátlapról az előlapra
        tween.TweenCallback(Callable.From(() => SwapFace()));

        // 3. Mozgatás a célcellára, és ezzel PÁRHUZAMOSAN a kártya szétnyitása a célméretre
        tween.TweenProperty(this, "global_position", targetGlobalPos, MoveDuration);
        tween.Parallel().TweenProperty(this, "scale:x", targetScale.X, half);
        tween.Parallel().TweenProperty(this, "scale:y", targetScale.Y, half);

        return tween;

    }

    private void SwapFace()
    {
        _backFace.Visible = false;
        FrontFace.Visible = true;
    }
}