using System.Collections.Generic;
using System;
using Godot;

public abstract partial class CardHolderBase
{
	public List<CardBase> _cardNodes = new List<CardBase>();
	protected string _name = "placeholder";
	private int _score = 0;
	private CardContainer cardContainer;

	private int Score
	{
		get => _score;
		set
		{
			_score = value;
			UpdateLabel();
		}
	}

	private RichTextLabel _label;


	public CardHolderBase(string name, int score, RichTextLabel label, CardContainer container) { _name = name; _label = label; Score = score; cardContainer = container; }


	

	private void UpdateLabel()
	{
		_label.Text = _name + "\n" + Score + " pont";
	}
	protected int whichCell(int value)
	{
		if (value >= 1 && value <= 8) return 0;
		else if (value >= 11 && value <= 18) return 1;
		else if (value >= 21 && value <= 28) return 2;
		else if (value >= 31 && value <= 38) return 3;
		else throw new ArgumentException("nem ide valo value");
	}
	public int getCardsInHandCount() { return _cardNodes.Count; }

	protected bool isPlaceable(int value, int startingCardValue, Cell cell)
	{
		return value % 10 == startingCardValue || value == cell.getValue() + 1 || value % 10 == 1 && cell.getValue() % 10 == 8;

	}
	public void onWin(int winsum)
	{
		GD.Print(_name + " nyert!");
		Score += winsum;
	}
	public void onLose()
	{
		Score -= getCardsInHandCount();
	}
	public void resetState()
	{
		_cardNodes = new List<CardBase>();
		foreach (CardBase item in cardContainer.GetChildren())
		{
			item.QueueFree();
		}
	}

	public void deal(List<int> used, List<(int, Texture2D)> cardData)
	{


		Random random = new Random();
		for (int i = 0; i < 8; i++)
		{
			CardBase newcard = (CardBase)cardContainer.CardScene.Instantiate();
			cardContainer.AddChild(newcard);
			_cardNodes.Add(newcard);
			int rnd = random.Next(0, 32);
			while (used.Contains(rnd))
			{
				rnd = random.Next(0, 32);
			}
			used.Add(rnd);
			_cardNodes[i].setDatas(cardData[rnd].Item1, cardData[rnd].Item2);

		}

	}
}
