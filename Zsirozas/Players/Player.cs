using Godot;
using System;
using System.Collections.Generic;

namespace zsir;

public partial class Player : EntityBase
{
	public Player(string name,int id, CardContainer container) : base(name,id, container) { DisableCards(); }

	public int StartRound( PlayerCard clickedCard)
	{
		DisableCards();
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();


		clickedCard.Animate(Name,  Zsir.GameAreaCell, () =>
			{
				Zsir.GameAreaCell.setDatas(value, texture);
				CardsInHands.Remove(clickedCard);
				clickedCard.QueueFree();
			});
		return value;
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

	public void NormalRound(PlayerCard clickedCard)
	{
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();

		DisableCards();
		clickedCard.Animate(Name,  Zsir.GameAreaCell, () =>
		{
			Zsir.GameAreaCell.setDatas(value, texture);
			CardsInHands.Remove(clickedCard);
			clickedCard.QueueFree();
		});
	}
}
