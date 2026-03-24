using Godot;
using System;
using System.Collections.Generic;

namespace zsir;

public partial class Bot : CardHolderBase
{
	public Bot(string name,int id, int score, CardContainer container, Cell area) : base(name,id, score, container, area) { }

	public CardBase startRound(ref int startingCardValue)
	{
		isStartingPlayer = true;
		Random random = new Random();
		int whichCard = random.Next(0, _cardNodes.Count);

		int value = _cardNodes[whichCard].getValue();
		Texture2D texture = _cardNodes[whichCard].getTexture();


		BackCard playedCard = (BackCard)_cardNodes[whichCard];


		playedCard.Animate(_name, gameArea, () =>
				{
					gameArea.setDatas(value, texture);
					_cardNodes.Remove(playedCard);
					playedCard.deleteCard();


				});

		startingCardValue = value;
		return playedCard;


	}
	public void endRound()
    {
        
    }

	public CardBase normalRound( int startingCardValue)
	{
		for (int i = 0; i < _cardNodes.Count; i++)
		{
			int value = _cardNodes[i].getValue();
			Texture2D texture = _cardNodes[i].getTexture();



			if (isSameType(value, startingCardValue))
			{
				BackCard playedCard = (BackCard)_cardNodes[i];


				playedCard.Animate(_name, gameArea, () =>
				{
					gameArea.setDatas(value, texture);
					_cardNodes.Remove(playedCard);
					playedCard.deleteCard();


				});



				return playedCard;
			}

		}
		//ha nincs ugyan olyan tipusu, random választ egyet
		Random random = new Random();
		int randomi = random.Next(0, _cardNodes.Count);
		BackCard selectedCard = (BackCard)_cardNodes[randomi];
		int val = _cardNodes[randomi].getValue();
		Texture2D text = _cardNodes[randomi].getTexture();

		selectedCard.Animate(_name, gameArea, () =>
		{
			gameArea.setDatas(val, text);
			_cardNodes.Remove(selectedCard);
			selectedCard.deleteCard();


		});

		
		return selectedCard;
	}


}
