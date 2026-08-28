using System.Collections.Generic;
using Godot;
using zsir;

namespace cardgames.Zsirozas.Players;

public class Player : EntityBase
{
    public Player(string name, int id, CardContainer container) : base(name, id, container)
    {
        DisableCards();
    }

    public int PlayRound(PlayerCard clickedCard)
    {
        int value = clickedCard.getValue();
        Texture2D texture = clickedCard.getTexture();
        DisableCards();
        PlayCardSound();
        clickedCard.Animate(Name, Zsir.GameAreaCell, () =>
        {
            Zsir.GameAreaCell.setDatas(value, texture);
            CardsInHands.Remove(clickedCard);
            clickedCard.QueueFree();
        });
        return value;
    }
    /*public void NormalRound(PlayerCard clickedCard)
    {
        int value = clickedCard.getValue();
        Texture2D texture = clickedCard.getTexture();

        DisableCards();
        clickedCard.Animate(Name, Zsir.GameAreaCell, () =>
        {
            Zsir.GameAreaCell.setDatas(value, texture);
            CardsInHands.Remove(clickedCard);
            clickedCard.QueueFree();
        });
    }*/

    public void EnableAllCards()
    {
        foreach (PlayerCard card in CardsInHands)
        {
            card.enableCard();
        }
    }

    public void DisableCards()
    {
        foreach (PlayerCard card in CardsInHands)
        {
            card.disableCard();
        }
    }

    public bool DoHavePlayableCard()
    {
        foreach (var card in CardsInHands)
        {
            if (IsSameType(card.getValue(), ZsirGameLogic.StartingCardValue) || IsVII(card.getValue())) return true;
        }

        return false;
    }

    public void EnablePlayableCards()
    {
        foreach (PlayerCard card in CardsInHands)
        {
            if (IsSameType(card.getValue(), ZsirGameLogic.StartingCardValue) || IsVII(card.getValue()))
                card.enableCard();
        }
    }

  
}