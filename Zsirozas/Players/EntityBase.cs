using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace zsir;

public partial class EntityBase : Node
{
    public List<CardBase> CardsInHands {get; private set;} = new();
    public int Id { get; }
    public new string Name { get; }
    public List<CardBase> CollectedCards = new List<CardBase>();
    private CardContainer _cardContainer;

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
        foreach (CardBase item in _cardContainer.GetChildren())
        {
            item.QueueFree();
        }
    }

    public void NewRoundDeal(ZsirGameLogic gameLogic,int maxCardNumForPlayer)
    {
        int missingCards = 4 - CardsInHands.Count;
        
        int cardsToPull = Math.Min(maxCardNumForPlayer, missingCards);

        for (int i = 0; i < cardsToPull; i++)
        {
            int rnd = gameLogic.DrawCardIndex();
            if (rnd == -1) throw new Exception(); // Ha elfogytak a lapok

            CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
            _cardContainer.AddChild(newcard);
            CardsInHands.Add(newcard);
            CardsInHands.Last().setDatas(CardDatabase.CardDatas[rnd].Item1, CardDatabase.CardDatas[rnd].Item2);
        }

        //if (CardsInHands.Count < 4) NewRoundDeal(gameLogic);
    }

    public void NewGameDeal(ZsirGameLogic gameLogic)
    {
        for (int i = 0; i < 4; i++)
        {
            CardBase newcard = (CardBase)_cardContainer.CardScene.Instantiate();
            _cardContainer.AddChild(newcard);
            CardsInHands.Add(newcard);
            int rnd = gameLogic.DrawCardIndex();
            CardsInHands[i].setDatas(CardDatabase.CardDatas[rnd].Item1, CardDatabase.CardDatas[rnd].Item2);
        }

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