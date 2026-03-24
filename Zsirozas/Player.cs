using Godot;
using System;
using System.Collections.Generic;

namespace zsir;

public partial class Player : CardHolderBase
{
	public Player(string name,int id, int score, CardContainer container, Cell area) : base(name,id, score, container, area) { disableCards(); }




	public List<CardBase> getList()
	{
		return _cardNodes;
	}

	public void startRound(List<Cell> cells, ref int startingCardValue, PlayerCard clickedCard)
	{
		disableCards();
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();


		clickedCard.Animate(_name, gameArea, () =>
			{
				gameArea.setDatas(value, texture);
				_cardNodes.Remove(clickedCard);
				clickedCard.QueueFree();
			});
		startingCardValue = value;
	}
	internal void enableCards()
	{
		foreach (PlayerCard card in _cardNodes) { card.enableCard(); }
	}
	internal void disableCards()
	{
		foreach (PlayerCard card in _cardNodes) { card.disableCard(); }
	}

	public void normalRound(List<Cell> cells, int startingCardValue, PlayerCard clickedCard)
	{
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();

		disableCards();
		clickedCard.Animate(_name, gameArea, () =>
		{
			gameArea.setDatas(value, texture);
			_cardNodes.Remove(clickedCard);
			clickedCard.QueueFree();
		});
	}
}
