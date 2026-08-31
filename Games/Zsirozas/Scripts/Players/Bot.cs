using System;
using System.Threading.Tasks;
using cardgames.Games.Zsirozas.Scripts.Cards;
using cardgames.Zsirozas;
using Godot;
using zsir;

namespace cardgames.Games.Zsirozas.Scripts.Players;

public class Bot : EntityBase
{
    public Bot(string name, int id, CardContainer container) : base(name, id, container)
    {
    }

    public async Task<CardBase> StartRound()
    {
        Random random = new Random();
        int whichCard = random.Next(0, CardsInHands.Count);
        BackCard playedCard = (BackCard)CardsInHands[whichCard];
        await PlayCard(playedCard);
        return playedCard;
    }
    //-1 startingcardvalue ha nincs kezdő lap még
    //3 lehetőség: kezdő, muszaj raknia, nem muszaj raknia
    public CardBase SelectPlayedCard(int startingCardValue, bool mustPlay = false)
    {
        if (startingCardValue != -1)
        {
            
            for (int i = 0; i < CardsInHands.Count; i++)
            {
                int value = CardsInHands[i].getValue();

                if (IsSameType(value, startingCardValue))
                {
                    return (BackCard)CardsInHands[i];
                }
            }
        }
        if (startingCardValue == -1 || mustPlay)
        {
            Random random = new Random();
            int randomCard = random.Next(0, CardsInHands.Count);
            return (BackCard)CardsInHands[randomCard];
        } 
        return null;
 
    }


    public async Task<CardBase> NormalRound(int startingCardValue, bool mustPlay)
    {
        for (int i = 0; i < CardsInHands.Count; i++)
        {
            int value = CardsInHands[i].getValue();

            if (IsSameType(value, startingCardValue))
            {
                BackCard playedCard = (BackCard)CardsInHands[i];
                await PlayCard(playedCard);
                return playedCard;
            }
        }

        if (mustPlay)
        {
            //ha nincs ugyan olyan tipusu, random választ egyet
            Random random = new Random();
            int randomi = random.Next(0, CardsInHands.Count);
            BackCard selectedCard = (BackCard)CardsInHands[randomi];
            await PlayCard(selectedCard);
            return selectedCard;
        }
        else return null;
    }

 
}