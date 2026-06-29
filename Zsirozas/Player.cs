using Godot;
using System;
using System.Collections.Generic;

namespace zsir;

public partial class Player : EntityBase
{
	public Player(string name,int id, CardContainer container, Cell area) : base(name,id, container, area) { DisableCards(); }

	public void StartRound(List<Cell> cells, ref int startingCardValue, PlayerCard clickedCard)
	{
		DisableCards();
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();


		clickedCard.Animate(Name, GameArea, () =>
			{
				GameArea.setDatas(value, texture);
				CardsInHands.Remove(clickedCard);
				clickedCard.QueueFree();
			});
		startingCardValue = value;
	}
	public void EnableCards()
	{
		foreach (PlayerCard card in CardsInHands) { card.enableCard(); }
	}
	public void DisableCards()
	{
		foreach (PlayerCard card in CardsInHands) { card.disableCard(); }
	}

	public bool IsHavePlayableCard(int startValue)
	{
		foreach (var card in CardsInHands)
		{
			if(IsSameType(card.getValue(),startValue) || IsVII(card.getValue())) return true;
		}

		return false;
	}

	public void NormalRound(List<Cell> cells, int startingCardValue, PlayerCard clickedCard)
	{
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();

		DisableCards();
		clickedCard.Animate(Name, GameArea, () =>
		{
			GameArea.setDatas(value, texture);
			CardsInHands.Remove(clickedCard);
			clickedCard.QueueFree();
		});
	}
}
