using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cardgames.Games._21.Scripts.Cards;
using cardgames.Games.Zsirozas.Scripts.UI;
using Godot;

namespace cardgames.Games._21.Scripts.Players;

public class EntityBase
{
      public List<CardBase> CardsInHands { get; private set; } = new();
    public int Id { get; }
    public string Name { get; }

    private readonly CardContainer _cardContainer;
    private const int WaitMillisAfterCardPlace = 400;
    public CardContainer CardContainer => _cardContainer;

    private readonly AudioStreamPlayer _soundPlayer;
    private readonly AudioStream _cardPlaceSound;

    private int  _originalMoney;
    public int Money { get; set; }

    
    protected EntityBase(string name, int id, CardContainer container, int money)
    {
        Name = name;
        Id = id;
        _originalMoney = money;
        Money = money;
        _cardContainer = container;
        _cardPlaceSound = GD.Load<AudioStream>("res://Assets/Sound/card_placed.mp3");
        
        _soundPlayer = new AudioStreamPlayer();
        _soundPlayer.Stream = _cardPlaceSound;
        _cardContainer.AddChild(_soundPlayer);    
    }

    private void PlayCardSound()
    {
        if (_soundPlayer == null || _cardPlaceSound == null) return;
        _soundPlayer.PitchScale = (float)GD.RandRange(0.95, 1.05);
        _soundPlayer.Play();
    }


    public void ResetRoundState()
    {
        CardsInHands.Clear();
        foreach (CardBase item in _cardContainer.GetChildren().OfType<CardBase>())
        {
            item.QueueFree();
        }
    }

    public void ResetGameState()
    {
        Money = _originalMoney;
        //ResetRoundState();
    }

    public void NewCardToHand(int random)
    {
        CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
        _cardContainer.AddChild(newcard);
        CardsInHands.Add(newcard);
        // int rnd = gameLogic.DrawCardIndex();
        CardsInHands.Last()
            .setDatas(CardDatabase.CardDatas[random].Item1, CardDatabase.CardDatas[random].Item2);

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
    
    public async Task PlayCard(CardBase playedCard)
    {
  
        PlayCardSound();
        Tween tween = playedCard.Animate(Name,  Zsir.GameAreaCell);
        await Zsir.GameAreaCell.ToSignal(tween, Tween.SignalName.Finished);
        Zsir.GameAreaCell.setDatas(playedCard.getValue(), playedCard.getTexture());
        CardsInHands.Remove(playedCard);
        playedCard.deleteCard();
        await Task.Delay(WaitMillisAfterCardPlace);
    }
}