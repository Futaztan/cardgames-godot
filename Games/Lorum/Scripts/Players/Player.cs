using System.Collections.Generic;
using System.Threading.Tasks;
using cardgames.Games.Lorum.Scripts.Cards;
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
        return CardsInHand;
    }

    public async Task<int> StartRound(PlayerCard clickedCard)
    {
        DisableCards();
        await PlayCard(clickedCard);
        return clickedCard.getValue();
    }

    public void EnableCards()
    {
        foreach (PlayerCard card in CardsInHand)
        {
            card.EnableCard();
        }
    }

    public void DisableCards(PlayerCard exceptWithOutColor = null)
    {
        foreach (PlayerCard card in CardsInHand)
        {
            if (card.Equals(exceptWithOutColor))
            {
                card.DisableCard(false);
            }
            else card.DisableCard(true);
        }
    }

    public async Task<int> NormalRound(PlayerCard clickedCard)
    {
        int value = clickedCard.getValue();
        Cell cell = Lorum.CenterCells[WhichCell(value)];

        if (IsPlaceable(value, cell))
        {
            DisableCards(clickedCard);
            await PlayCard(clickedCard);
            return CardsInHand.Count;
        }

        return -1;
    }

    public override async Task PassTurn()
    {
        Container box = (Container)CardsInHand[0].GetParent().GetParent();
        Vector2 pos = box.GlobalPosition + new Vector2(box.Size.X * 0.5f - _passIcon.Size.Y * 0.5f, 0);
        await _passIcon.MoveTo(pos);
    }
}