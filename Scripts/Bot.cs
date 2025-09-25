using Godot;
using System;
using System.Collections.Generic;


public class Bot : CardHolderBase
{

	

public Bot(string name, int score, RichTextLabel label, CardContainer container) : base(name, score, label, container){}

	

	


	public void startRound(List<Cell> cells, ref int startingCardValue)
	{
		Random random = new Random();
		int whichCard = random.Next(0, 8);

		int value = _cardNodes[whichCard].getValue();
		Texture2D texture = _cardNodes[whichCard].getTexture();
		Cell cell = cells[whichCell(value)];

		BackCard playedCard = (BackCard)_cardNodes[whichCard];


		playedCard.Animate(_name, cell, () =>
				{
					cell.setDatas(value, texture);
					_cardNodes.Remove(playedCard);
					playedCard.deleteCard();


				});

		startingCardValue = value;


	}

	public int normalRound(List<Cell> cells, int startingCardValue)
	{
		for (int i = 0; i < _cardNodes.Count; i++)
		{
			int value = _cardNodes[i].getValue();
			Texture2D texture = _cardNodes[i].getTexture();
			Cell cell = cells[whichCell(value)];


			if (isPlaceable(value, startingCardValue, cell))
			{
				BackCard playedCard = (BackCard)_cardNodes[i];


				playedCard.Animate(_name, cell, () =>
				{
					cell.setDatas(value, texture);
					_cardNodes.Remove(playedCard);
					playedCard.deleteCard();


				});



				return _cardNodes.Count - 1;
			}

		}
		GD.Print("bot passz");
		passTurn();
		return -1;
	}

	private void passTurn()
	{

		BoxContainer box = (BoxContainer)_cardNodes[0].GetParent();
		Vector2 pos;
		switch (_name)
		{
			case "bot1": pos = box.GlobalPosition + new Vector2(box.Size.X, box.Size.Y * 0.5f); break;
			case "bot2": pos = box.GlobalPosition + new Vector2(box.Size.X * 0.5f, box.Size.Y); break;
			case "bot3": pos = box.GlobalPosition + new Vector2(0, box.Size.Y * 0.5f); break;
			default: throw new Exception();

		}
		Lorum.passIcon.moveTo(pos);



	}






}
