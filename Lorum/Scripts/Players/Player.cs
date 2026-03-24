using Godot;
using System.Collections.Generic;

namespace lorum;
public partial class Player : CardHolderBase
{

	public Player(string name, int score, RichTextLabel label, CardContainer container) : base(name, score, label, container) { disableCards(); }



	
	public List<CardBase> getList()
	{
		return _cardNodes;
	}

	public void startRound(List<Cell> cells, ref int startingCardValue, PlayerCard clickedCard)
	{
		disableCards();
		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();
		Cell cell = cells[whichCell(value)];

		clickedCard.Animate(_name, cell, () =>
			{
				cell.setDatas(value, texture);
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

	public int normalRound(List<Cell> cells, int startingCardValue, PlayerCard clickedCard)
	{


		int value = clickedCard.getValue();
		Texture2D texture = clickedCard.getTexture();
		Cell cell = cells[whichCell(value)];

		if (isPlaceable(value, startingCardValue, cell))
		{
			disableCards();
			clickedCard.Animate(_name, cell, () =>
			{
				cell.setDatas(value, texture);
				_cardNodes.Remove(clickedCard);
				clickedCard.QueueFree();
			});


			return _cardNodes.Count - 1;

		}
		return -1;

	}

	internal bool canPlaceCard(List<Cell> cells, int startingCardValue)
	{
		for (int i = 0; i < _cardNodes.Count; i++)
		{
			PlayerCard card = (PlayerCard)_cardNodes[i];
			int value = card.getValue();
			Cell cell = cells[whichCell(value)];

			if (isPlaceable(value, startingCardValue, cell)) return true;
		}
		passTurn();
		return false;
	}

	private void passTurn()
	{
		Container box = (Container)_cardNodes[0].GetParent().GetParent();
		Vector2 pos = box.GlobalPosition + new Vector2(box.Size.X * 0.5f - Lorum.passIcon.Size.Y*0.5f, 0);
		Lorum.passIcon.moveTo(pos);
	}






}
