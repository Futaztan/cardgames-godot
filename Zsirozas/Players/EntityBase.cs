using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace zsir;

public class EntityBase 
{
    public List<CardBase> CardsInHands { get; private set; } = new();
    public int Id { get; }
    public string Name { get; }
    public List<CardBase> CollectedCards = new List<CardBase>();
    private CardContainer _cardContainer;

    public CardContainer CardContainer => _cardContainer;

    private AudioStreamPlayer _soundPlayer;
    private AudioStream _cardPlaceSound;

    public int Score
    {
        get
        {
            int score = 0;
            foreach (var card in CollectedCards)
            {
                if (card.getValue() % 10 == 4 || card.getValue() % 10 == 8) //ÁSZ VAGY X
                {
                    score += 10;
                }
            }

            return score;
        }
    }


    protected EntityBase(string name, int id, CardContainer container)
    {
        Name = name;
        Id = id;
        _cardContainer = container;
        _cardPlaceSound = GD.Load<AudioStream>("res://Assets/Sound/card_placed.mp3");
        
        _soundPlayer = new AudioStreamPlayer();
        _soundPlayer.Stream = _cardPlaceSound;
        _cardContainer.AddChild(_soundPlayer);    
    }

    protected void PlayCardSound()
    {
        if (_soundPlayer == null || _cardPlaceSound == null) return;
        _soundPlayer.PitchScale = (float)GD.RandRange(0.95, 1.05);
        _soundPlayer.Play();
    }

    protected bool IsSameType(int value, int startingCardValue)
    {
        return (value % 10) == (startingCardValue % 10);
    }

    protected bool IsVII(int value)
    {
        return (value % 10 == 5);
    }


    public void ResetState()
    {
        CardsInHands.Clear();
        CollectedCards.Clear();
        foreach (CardBase item in _cardContainer.GetChildren().OfType<CardBase>())
        {
            item.QueueFree();
        }
    }

    public void NewCardToHand(int random)
    {
        CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
        _cardContainer.AddChild(newcard);
        CardsInHands.Add(newcard);
        // int rnd = gameLogic.DrawCardIndex();
        CardsInHands.Last()
            .setDatas(CardDatabase.CardDatas[random].Item1, CardDatabase.CardDatas[random].Item2); //TODO BUGOS E

        GD.Print("-------------");
        GD.Print(Name);
        foreach (var item in CardsInHands)
        {
            _cardContainer.RemoveChild(item);
        }

        CardsInHands = CardsInHands.OrderBy(node => node.getValue()).ToList();
        foreach (var item in CardsInHands)
        {
            GD.Print(item.getValue() + " " + item.getTexture());
            _cardContainer.AddChild(item);
        }
    }
}