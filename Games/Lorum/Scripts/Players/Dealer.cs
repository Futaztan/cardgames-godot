using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Players;

public class Dealer
{
    private readonly List<EntityBase> _allPlayers;
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
        if (_usedCardIndexes.Count >= 32) throw new IndexOutOfRangeException(); // Elfogyott a pakli
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

    public async Task DealCardsToPlayers()
    {
        Node mainScene = GetMainScene();
        TextureRect cardDeck = mainScene.GetNode<TextureRect>("%CardDeck");
        cardDeck.Visible = true;
        for (int i = 0; i < 8; i++)
        {
            foreach (var entity in _allPlayers)
            {
                DealAnimation(mainScene, cardDeck, entity.CardContainer, () =>
                {
                    int rnd = DrawCardIndex();
                    entity.NewCardToHand(rnd);
                });

                await mainScene.ToSignal(mainScene.GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
            }
        }

        cardDeck.Visible = false;
        await mainScene.ToSignal(mainScene.GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
    }

    private Node GetMainScene()
    {
        SceneTree tree = (SceneTree)Engine.GetMainLoop();
        return tree.CurrentScene;
    }

    private void DealAnimation(Node mainScene, Control cardDeck, Control toNode, Action onAnimationDone)
    {
        TextureRect animatedCard = new TextureRect();
        animatedCard.Texture = GD.Load<Texture2D>("res://Assets/Cards/back.png");
        animatedCard.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        animatedCard.StretchMode = TextureRect.StretchModeEnum.KeepAspect;
        animatedCard.Size = 1.1f * cardDeck.Size;
        mainScene.AddChild(animatedCard);
        animatedCard.GlobalPosition = cardDeck.GlobalPosition;
        animatedCard.PivotOffset = animatedCard.Size / 2.0f;
        _soundPlayer.PitchScale = (float)GD.RandRange(0.90, 1.05);
        _soundPlayer.Play();

        Tween tween = mainScene.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(animatedCard, "global_position",
            toNode.GlobalPosition + new Vector2(toNode.Size.X / 2f, 0) - new Vector2(animatedCard.Size.X / 2f, 0),
            0.5f);
        //tween.Parallel().TweenProperty(repuloKartya, "rotation_degrees", 360.0f, 0.5f);

        tween.TweenCallback(Callable.From(() =>
        {
            animatedCard.QueueFree();
            onAnimationDone?.Invoke();
        }));
    }
}