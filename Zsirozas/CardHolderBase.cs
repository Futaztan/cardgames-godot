using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace zsir;
public partial class CardHolderBase : Node
{
    
	public List<CardBase> _cardNodes = new List<CardBase>();
	public int Id { get;}
	protected string _name = "placeholder";
	public List<CardBase> takenCards = new List<CardBase>();

	private int _score = 0;
	private CardContainer cardContainer;

	protected Cell gameArea;
	protected bool isStartingPlayer = false;

	protected static List<int> usedCardIndex = new List<int>();
	

	


	public CardHolderBase(string name, int id, int score, CardContainer container, Cell area)
	{ _name = name;Id = id; _score = score; cardContainer = container; gameArea = area;}

	public int getScore() { return _score; }
	public string getName() { return _name; }


	
	
	
	public int getCardsInHandCount() { return _cardNodes.Count; }

	protected bool isSameType(int value, int startingCardValue)
	{
		return (value % 10 ) == (startingCardValue % 10);

	}
	public void onWin()
	{
		
	}
	public void onLose()
	{

	}
	public void resetState()
	{
		_cardNodes = new List<CardBase>();
		foreach (CardBase item in cardContainer.GetChildren())
		{
			item.QueueFree();
		}
	}

	public void newRoundDeal()
    {
        Random random = new Random();
		//int cardCount = 32 - usedCardIndex.Count;
		//int osztandoCount = 4 - _cardNodes.Count;

		for (int i = 0; i < 1; i++)
		{
			CardBase newcard = (CardBase)cardContainer.CardScene.Instantiate();
			cardContainer.AddChild(newcard);
			_cardNodes.Add(newcard);
			int rnd = random.Next(0, 32);
			while (usedCardIndex.Contains(rnd))
			{
				rnd = random.Next(0, 32);
			}
			usedCardIndex.Add(rnd);
			_cardNodes.Last().setDatas(Zsir.CardDatas[rnd].Item1, Zsir.CardDatas[rnd].Item2);
			
		}
    }

	public void deal()
	{


		Random random = new Random();
		for (int i = 0; i < 4; i++)
		{
			CardBase newcard = (CardBase)cardContainer.CardScene.Instantiate();
			cardContainer.AddChild(newcard);
			_cardNodes.Add(newcard);
			int rnd = random.Next(0, 32);
			while (usedCardIndex.Contains(rnd))
			{
				rnd = random.Next(0, 32);
			}
			usedCardIndex.Add(rnd);
			_cardNodes[i].setDatas(Zsir.CardDatas[rnd].Item1, Zsir.CardDatas[rnd].Item2);
			
		}
		GD.Print("-------------");
		GD.Print(_name);
		foreach (var item in _cardNodes)
		{
			
			cardContainer.RemoveChild(item);
		}
		_cardNodes =  _cardNodes.OrderBy(node => node.getValue()).ToList();
		foreach (var item in _cardNodes)
		{
			GD.Print(item.getValue() + " " + item.getTexture());
			cardContainer.AddChild(item);
		}

		if(Id==0)
        {
            foreach (var item in _cardNodes)
			{
				GD.Print(item.getValue());
			}
        }
		

	}
}
