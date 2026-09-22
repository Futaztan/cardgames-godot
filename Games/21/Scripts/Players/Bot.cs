using System;
using cardgames.Games._21.Scripts.Cards;
using Godot;

namespace cardgames.Games._21.Scripts.Players;

public class Bot  : EntityBase
{
    public Bot(string name, int id, CardContainer container, int money, Label label, Label valueLabel, GoldCoin goldCoin) : base(name, id, container, label, valueLabel, goldCoin)
    {
        PackedScene cardScene = GD.Load<PackedScene>("res://Games/21/Scenes/Cards/BackCard.tscn");
        Control cardInstance = (Control)cardScene.Instantiate();
        CardSize = cardInstance.Size;
        cardInstance.QueueFree();
    }

    public bool DecideToGetNewCard()
    {
        if (CardsValueInHand < 15)
        {
            return true;
        }
        else if (CardsValueInHand >= 15 && CardsValueInHand < 18)
        {
            Random random = new();
            int num = random.Next(0, 101);
            if (num < 50) return true;
            else return false;
        }
        else return false;
    }

    public void ShowCards()
    {
        foreach (BackCard card in CardsInHands)
        {
            card.AnimateSwapFace();
        }
    }

    public int SpecifyMaxWager()
    {
        //return 100;
        return Math.Min(Money, 20);
    }
}