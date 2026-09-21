using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using cardgames.Games._21.Scripts.Cards;
using Godot;
using Vector2 = Godot.Vector2;

namespace cardgames.Games._21.Scripts.Players;

public class Dealer
{
    private List<EntityBase> _allPlayers;
    private int DeckSize => 32 - _usedCardIndexes.Count;
    private readonly List<int> _usedCardIndexes = new List<int>();
    private readonly AudioStreamPlayer _soundPlayer;

    public Dealer(List<EntityBase> allPlayers)
    {
        _allPlayers = allPlayers;
        AudioStream cardPlaceSound = GD.Load<AudioStream>("res://Assets/Sound/card_deal.ogg");
        _soundPlayer = new AudioStreamPlayer();
        _soundPlayer.Stream = cardPlaceSound;
        GetMainScene().AddChild(_soundPlayer);
    }

    private int DrawCardIndex()
    {
        if (_usedCardIndexes.Count >= 32) return -1; // Elfogyott a pakli
        Random random = new();
        int rnd = random.Next(0, 32);
        while (_usedCardIndexes.Contains(rnd))
        {
            rnd = random.Next(0, 32);
        }

        _usedCardIndexes.Add(rnd);
        return rnd;
    }

    public void Reset()
    {
        _usedCardIndexes.Clear();
    }

    public async Task DealCard(EntityBase to, bool rotate)
    {
        Node mainScene = GetMainScene();
        Control cardDeck = mainScene.GetNode<Control>("%CardDeck");
        int rnd = DrawCardIndex();
       
        Tween tween = DealAnimation(mainScene, cardDeck, to, rotate, rnd, () =>
        {
            to.NewCardToHand(rnd);
        });

        await mainScene.ToSignal(tween, Tween.SignalName.Finished);
    }

    private Node GetMainScene()
    {
        SceneTree tree = (SceneTree)Engine.GetMainLoop();
        return tree.CurrentScene;
    }

    private Tween DealAnimation(Node mainScene, Control cardDeck, EntityBase to, bool rotate, int randomValue, Action onAnimationDone)
    {
        
        TextureRect animatedCard = new TextureRect();
        animatedCard.Texture = GD.Load<Texture2D>("res://Assets/Cards/back.png");
        animatedCard.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        animatedCard.StretchMode = TextureRect.StretchModeEnum.Scale;
        animatedCard.Size = to.CardSize;
        mainScene.AddChild(animatedCard);
        animatedCard.GlobalPosition = cardDeck.GlobalPosition;
        animatedCard.PivotOffset = animatedCard.Size / 2.0f;
        _soundPlayer.PitchScale = (float)GD.RandRange(0.90, 1.05);
        _soundPlayer.Play();
        Tween tween = mainScene.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        if (rotate)
        {
            // 1. Kártya becsukása X tengelyen (Flip első fele)
            tween.TweenProperty(animatedCard, "scale:x", 0f, 0.25f);

            // 2. Kép átváltása a hátlapról az előlapra
            tween.TweenCallback(Callable.From(() =>
            {
               animatedCard.Texture = CardDatabase.CardDatas[randomValue].Texture;
            }));
        }

        tween.TweenProperty(animatedCard, "global_position",
            to.CardContainer.GlobalPosition + new Vector2(to.CardContainer.Size.X / 2f, 0) - new Vector2(animatedCard.Size.X / 2f, 0),
            0.5f);

        if (rotate)
        {
            Vector2 targetScale = new Vector2(190, 314) / animatedCard.Size;
            tween.Parallel().TweenProperty(animatedCard, "scale:x", targetScale.X, 0.5f);
            tween.Parallel().TweenProperty(animatedCard, "scale:y", targetScale.Y, 0.25f);
        }

        tween.Finished += () =>
        {
            animatedCard.QueueFree();
            onAnimationDone?.Invoke();
        };
        return tween;
    }
}