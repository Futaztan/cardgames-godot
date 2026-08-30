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
        return CardNodes;
    }

    public async Task<int> StartRound(PlayerCard clickedCard)
    {
        DisableCards();
        int value = clickedCard.getValue();
        Texture2D texture = clickedCard.getTexture();
        Cell cell = Lorum.CenterCells[WhichCell(value)];
        PlayCardSound();
        Tween tween = clickedCard.Animate(_name, cell);
        
        await cell.ToSignal(tween, Tween.SignalName.Finished);
        cell.setDatas(value, texture);
        CardNodes.Remove(clickedCard);
        await Task.Delay(WaitMillisAfterCardPlace);
        clickedCard.deleteCard();
        return value;
    }

    public void EnableCards()
    {
        foreach (PlayerCard card in CardNodes)
        {
            card.EnableCard();
        }
    }

    public void DisableCards(PlayerCard exceptWithOutColor = null)
    {
        foreach (PlayerCard card in CardNodes)
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
        Texture2D texture = clickedCard.getTexture();
        Cell cell = Lorum.CenterCells[WhichCell(value)];

        if (IsPlaceable(value, cell))
        {
            DisableCards(clickedCard);
            PlayCardSound();
            Tween tween = clickedCard.Animate(_name, cell);
            await cell.ToSignal(tween, Tween.SignalName.Finished);
            cell.setDatas(value, texture);
            CardNodes.Remove(clickedCard);
            await Task.Delay(WaitMillisAfterCardPlace);
            clickedCard.deleteCard();
            return CardNodes.Count - 1;
        }

        return -1;
    }

    public override async Task PassTurn()
    {
        Container box = (Container)CardNodes[0].GetParent().GetParent();
        Vector2 pos = box.GlobalPosition + new Vector2(box.Size.X * 0.5f - _passIcon.Size.Y * 0.5f, 0);
        await _passIcon.MoveTo(pos);
    }
}