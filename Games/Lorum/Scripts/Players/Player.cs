using System.Collections.Generic;
using cardgames.Lorum.Scripts.UI;
using Godot;

namespace cardgames.Games.Lorum.Scripts.Players;

public class Player : EntityBase
{
    public Player(string name, int score, RichTextLabel label, CardContainer container, Pass passIcon) : base(name, score, label,
        container, passIcon)
    {
    
        DisableCards();
    }
    public List<CardBase> GetCardNodes()
    {
        return CardNodes;
    }

    public int StartRound(PlayerCard clickedCard)
    {
        DisableCards();
        int value = clickedCard.getValue();
        Texture2D texture = clickedCard.getTexture();
        Cell cell = Games.Lorum.Scripts.Lorum.CenterCells[WhichCell(value)];
        PlayCardSound();
        clickedCard.Animate(_name, cell, () =>
        {
            cell.setDatas(value, texture);
            CardNodes.Remove(clickedCard);
            clickedCard.QueueFree();
        });
        return value;
    }

    public void EnableCards()
    {
        foreach (PlayerCard card in CardNodes)
        {
            card.enableCard();
        }
    }

    public void DisableCards()
    {
        foreach (PlayerCard card in CardNodes)
        {
            card.disableCard();
        }
    }

    public int NormalRound(PlayerCard clickedCard)
    {
        int value = clickedCard.getValue();
        Texture2D texture = clickedCard.getTexture();
        Cell cell = Games.Lorum.Scripts.Lorum.CenterCells[WhichCell(value)];

        if (IsPlaceable(value, cell))
        {
            DisableCards();
            PlayCardSound();
            clickedCard.Animate(_name, cell, () =>
            {
                cell.setDatas(value, texture);
                CardNodes.Remove(clickedCard);
                clickedCard.QueueFree();
            });


            return CardNodes.Count - 1;
        }

        return -1;
    }

    public bool CanPlaceCard()
    {
        for (int i = 0; i < CardNodes.Count; i++)
        {
            PlayerCard card = (PlayerCard)CardNodes[i];
            int value = card.getValue();
            Cell cell = Games.Lorum.Scripts.Lorum.CenterCells[WhichCell(value)];

            if (IsPlaceable(value, cell)) return true;
        }

        PassTurn();
        return false;
    }

    private void PassTurn()
    {
        Container box = (Container)CardNodes[0].GetParent().GetParent();
        Vector2 pos = box.GlobalPosition + new Vector2(box.Size.X * 0.5f - _passIcon.Size.Y * 0.5f, 0);
        _passIcon.moveTo(pos);
    }
}