using Godot;
using System;
using System.Collections.Generic;

namespace zsir;

public partial class Bot : EntityBase
{
    public Bot(string name, int id, CardContainer container, Cell area) : base(name, id, container, area)
    {
    }

    public CardBase StartRound(ref int startingCardValue)
    {
        Random random = new Random();
        int whichCard = random.Next(0, CardsInHands.Count);

        int value = CardsInHands[whichCard].getValue();
        Texture2D texture = CardsInHands[whichCard].getTexture();


        BackCard playedCard = (BackCard)CardsInHands[whichCard];


        playedCard.Animate(Name, GameArea, () =>
        {
            GameArea.setDatas(value, texture);
            CardsInHands.Remove(playedCard);
            playedCard.deleteCard();
        });

        startingCardValue = value;
        return playedCard;
    }


    public CardBase NormalRound(int startingCardValue, bool mustPlay)
    {
        for (int i = 0; i < CardsInHands.Count; i++)
        {
            int value = CardsInHands[i].getValue();
            Texture2D texture = CardsInHands[i].getTexture();


            if (IsSameType(value, startingCardValue))
            {
                BackCard playedCard = (BackCard)CardsInHands[i];


                playedCard.Animate(Name, GameArea, () =>
                {
                    GameArea.setDatas(value, texture);
                    CardsInHands.Remove(playedCard);
                    playedCard.deleteCard();
                });


                return playedCard;
            }
        }

        if (mustPlay)
        {
            //ha nincs ugyan olyan tipusu, random választ egyet
            Random random = new Random();
            int randomi = random.Next(0, CardsInHands.Count);
            BackCard selectedCard = (BackCard)CardsInHands[randomi];
            int val = CardsInHands[randomi].getValue();
            Texture2D text = CardsInHands[randomi].getTexture();

            selectedCard.Animate(Name, GameArea, () =>
            {
                GameArea.setDatas(val, text);
                CardsInHands.Remove(selectedCard);
                selectedCard.deleteCard();
            });


            return selectedCard;
        }
        else return null;
    }
}