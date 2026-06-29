using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace zsir;

public class ZsirGameLogic
{
	public List<EntityBase> AllPlayers { get; private set; } = new();
	public List<Bot> Bots { get; private set; } = new();
	public Player HumanPlayer { get; private set; }
	public List<CardBase> CardsInArea { get; private set; } = new();

	public int WhoStarted { get; set; } = -1;
	public int StartingCardValue { get; set; } = -1; //maradékos osztás 10

	public EntityBase StartingPlayer => AllPlayers[WhoStarted];
	private List<int> _usedCardIndexes = new List<int>();
	
	public int DrawCardIndex()
	{
		if (_usedCardIndexes.Count >= 32) return -1; // Elfogyott a pakli
		Random random = new();
		int rnd = random.Next(0, 32);
		while (_usedCardIndexes.Contains(rnd))
		{
			rnd = random.Next(0, 32);
		}

		_usedCardIndexes.Add(rnd);
		return rnd;
	}

	public void SetupPlayers(Player player, List<Bot> bots)
	{
		HumanPlayer = player;
		Bots = bots;
		AllPlayers = new List<EntityBase> { player };
		AllPlayers.AddRange(bots);
	}

	public EntityBase EvaluateRoundWinner()
	{
		if (!CardsInArea.Any()) return null;

		EntityBase roundWinner = StartingPlayer;
		int starterCardValue = CardsInArea.First().getValue();

		for (int i = 0; i < CardsInArea.Count; i++)
		{
			int currentCardValue = CardsInArea[i].getValue();
			if (IsSameType(currentCardValue, starterCardValue) || IsVII(currentCardValue))
			{
				roundWinner = AllPlayers[(WhoStarted + i) % 4];
				GD.Print("JELENLEGI NYERTES: " + roundWinner.Name + " " + i);
			}
		}

		roundWinner.CollectedCards.AddRange(CardsInArea);
		return roundWinner;
	}

	public void ResetRoundState()
	{
		StartingCardValue = -1;
		CardsInArea.Clear();
	}

	public void ResetGameState()
	{
		_usedCardIndexes.Clear();
		ResetRoundState();
	}

	private bool IsSameType(int value, int startingCardValue)
	{
		return (value % 10) == (startingCardValue % 10);
	}

	private bool IsVII(int value)
	{
		if (value % 10 == 5) return true;
		return false;
	}

	public bool IsGameOver()
	{
		return AllPlayers.Sum(p => p.CardsInHands.Count) == 0;
	}

	public (EntityBase Winner, int PlayerPosition) GetGameResult()
	{
		int playerScore = HumanPlayer.Score;
		int position = 1;
		EntityBase winner = AllPlayers[0];
		int maxScore = winner.Score;

		for (int i = 1; i < AllPlayers.Count; i++)
		{
			int currentScore = AllPlayers[i].Score;
			if (maxScore < currentScore)
			{
				maxScore = currentScore;
				winner = AllPlayers[i];
			}

			if (playerScore < currentScore) position++;
		}

		return (winner, position);
	}
}
