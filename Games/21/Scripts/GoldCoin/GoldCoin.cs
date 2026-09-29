using Godot;
using System;
using System.Threading.Tasks;

public partial class GoldCoin : TextureRect
{
    public async void AnimateCoins(Control to)
    {
       
        for (int i = 0; i < 5; i++)
        {
            GoldCoin movingCoin = (GoldCoin) this.Duplicate();
            foreach (var child in movingCoin.GetChildren())
            {
                child.QueueFree();
            }
            var mainScene = GetMainScene();
            mainScene.AddChild(movingCoin);
            Tween tween = mainScene.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(movingCoin, "global_position",
                to.GlobalPosition + new Vector2(to.Size.X / 2f, 0) - new Vector2(movingCoin.Size.X / 2f, 0),
                0.5f);
            tween.Finished += movingCoin.QueueFree;
            await Task.Delay(100);
        }
    }
    
    private Node GetMainScene()
    {
        SceneTree tree = (SceneTree)Engine.GetMainLoop();
        return tree.CurrentScene;
    }
}
