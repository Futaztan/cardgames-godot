using System;
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
    private readonly Label _pointLabel;
    private readonly Label _cardsValueLabel;
    public GoldCoin GoldCoinTexture { get; private set; }

    private readonly AudioStreamPlayer _soundPlayer;
    private readonly AudioStream _cardPlaceSound;

    private int  _originalMoney;
    private int _money;

    public Vector2 CardSize { get; protected set; }

    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            _pointLabel.Text = _money.ToString();
            //_pointLabel.SetAnchorsPreset(Control.LayoutPreset.Center);
        }
    }

    protected EntityBase(string name, int id, CardContainer container, int money, Label label, Label valueLabel, GoldCoin goldCoin)
    {
        Name = name;
        Id = id;
        _originalMoney = money;
        _pointLabel = label;
        _cardsValueLabel = valueLabel;
        _cardContainer = container;
        GoldCoinTexture = goldCoin;
        Money = money;
        _cardPlaceSound = GD.Load<AudioStream>("res://Assets/Sound/card_placed.mp3");
        
        _soundPlayer = new AudioStreamPlayer();
        _soundPlayer.Stream = _cardPlaceSound;
        _cardContainer.AddChild(_soundPlayer);    
    }

    public int CardsValueInHand
    {
        get
        {
            int score = 0;
            foreach (var card in CardsInHands)
            {
                score += card.ScoreValue;
            }
            return score;
        }
    }
    public bool StartedWithTwoAces()
    {
        if (CardsInHands.Count < 2) return false;
        for (int i = 0; i < 2; i++)
        {
            if (CardsInHands[i].Value % 10 != 4) return false;
        }

        return true;
    }
    private void UpdateValueLabel()
    {
        int value = CardsValueInHand;
        if (StartedWithTwoAces()) value = 21;
       
        if (value > 21)
        {
            _cardsValueLabel.AddThemeColorOverride("font_color", new Color("#ff9999"));
        }
        else _cardsValueLabel.AddThemeColorOverride("font_color", new Color(Colors.White));

        _cardsValueLabel.Text = Name + " értéke: " + value;
    }

    public void SetLabelColor(string color)
    {
        _cardsValueLabel.AddThemeColorOverride("font_color", new Color(color));
    }

    private void PlayCardSound()
    {
        if (_soundPlayer == null || _cardPlaceSound == null) return;
        _soundPlayer.PitchScale = (float)GD.RandRange(0.95, 1.05);
        _soundPlayer.Play();
    }

    public void AnimateCoins(Control to)
    {
        GoldCoinTexture.AnimateCoins(to);
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
            .SetDatas(CardDatabase.CardDatas[random].Item1,CardDatabase.CardDatas[random].ScoreValue , CardDatabase.CardDatas[random].Item3);

        GD.Print("-------------");
        GD.Print(Name);
        /*foreach (var item in CardsInHands)
        {
            _cardContainer.RemoveChild(item);
        }

        CardsInHands = CardsInHands.OrderBy(node => node.Value).ToList();
        foreach (var item in CardsInHands)
        {
            GD.Print(item.Value + " " + item.GetTexture());
            _cardContainer.AddChild(item);
        }*/
        UpdateValueLabel();
    }
    
    public async Task PlayCard(CardBase playedCard)
    {
  
        PlayCardSound();
        Tween tween = playedCard.Animate(Name,  Zsir.GameAreaCell);
        await Zsir.GameAreaCell.ToSignal(tween, Tween.SignalName.Finished);
        Zsir.GameAreaCell.setDatas(playedCard.Value, playedCard.GetTexture());
        CardsInHands.Remove(playedCard);
        playedCard.DeleteCard();
        await Task.Delay(WaitMillisAfterCardPlace);
    }
}