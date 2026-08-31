using System.Threading.Tasks;
using cardgames.Games.Zsirozas.Scripts.Cards;
using cardgames.Zsirozas;
using Godot;
using zsir;

namespace cardgames.Games.Zsirozas.Scripts.Players;

public class Player : EntityBase
{
    public Player(string name, int id, CardContainer container) : base(name, id, container)
    {
        DisableCards();
    }
    
    public void EnableAllCards()
    {
        foreach (PlayerCard card in CardsInHands)
        {
            card.EnableCard();
        }
    }

    public void DisableCards(PlayerCard exceptWithOutColor = null)
    {
        foreach (PlayerCard card in CardsInHands)
        {
            if (card.Equals(exceptWithOutColor))
            {
                card.DisableCard(false);
            }
            else card.DisableCard(true);
        }
    }

    public bool HavePlayableCard()
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
                card.EnableCard();
        }
    }

  
}