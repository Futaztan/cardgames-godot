using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace zsir;

public partial class Zsir : Control
{
	private Bot _bot1;
	private Bot _bot2;
	private Bot _bot3;
	private Player _player;
	private List<Bot> _bots;
	private int _startingCardValue; //maradékos osztás 10
	private List<Cell> _cells = new List<Cell>();
	private StartingCardLabel _startingValueLabel;
	private Button testbutton;
	private List<CardHolderBase> _allPlayers;
	private int _whoStarted = -1;
	private CardHolderBase StartingPlayer
	{
		get
		{
			return _allPlayers[_whoStarted];
		}

	}
	private int _score;
	private List<CardBase> cardsInArea = new List<CardBase>();

	public static readonly List<(int, Texture2D)> CardDatas = new List<(int, Texture2D)>();

	private List<(int, string)> cardValuesPaths = new List<(int, string)>
	{

		(1, "res://Assets/Cards/zold_also.png"),
		(2, "res://Assets/Cards/zold_felso.png"),
		(3, "res://Assets/Cards/zold_kiraly.png"),
		(4, "res://Assets/Cards/zold_asz.png"),
		(5, "res://Assets/Cards/zold_7.png"),
		(6, "res://Assets/Cards/zold_8.png"),
		(7, "res://Assets/Cards/zold_9.png"),
		(8, "res://Assets/Cards/zold_10.png"),

		(11, "res://Assets/Cards/piros_also.png"),
		(12, "res://Assets/Cards/piros_felso.png"),
		(13, "res://Assets/Cards/piros_kiraly.png"),
		(14, "res://Assets/Cards/piros_asz.png"),
		(15, "res://Assets/Cards/piros_7.png"),
		(16, "res://Assets/Cards/piros_8.png"),
		(17, "res://Assets/Cards/piros_9.png"),
		(18, "res://Assets/Cards/piros_10.png"),

		(21, "res://Assets/Cards/makk_also.png"),
		(22, "res://Assets/Cards/makk_felso.png"),
		(23, "res://Assets/Cards/makk_kiraly.png"),
		(24, "res://Assets/Cards/makk_asz.png"),
		(25, "res://Assets/Cards/makk_7.png"),
		(26, "res://Assets/Cards/makk_8.png"),
		(27, "res://Assets/Cards/makk_9.png"),
		(28, "res://Assets/Cards/makk_10.png"),

		(31, "res://Assets/Cards/tok_also.png"),
		(32, "res://Assets/Cards/tok_felso.png"),
		(33, "res://Assets/Cards/tok_kiraly.png"),
		(34, "res://Assets/Cards/tok_asz.png"),
		(35, "res://Assets/Cards/tok_7.png"),
		(36, "res://Assets/Cards/tok_8.png"),
		(37, "res://Assets/Cards/tok_9.png"),
		(38, "res://Assets/Cards/tok_10.png")

	};


	//TODO diflabelek utan elbaszodik a label meret és nagyobb lesz,  ??? nem talaltam meg megint ezt


	/* 1. zold
	*  2. piros
	* 3. makk
	* 4. tok
	*
	*/


	//ha roundsuntilend = 0, majd -1 akkor amíg el nem fogynak a pontok
	public void init()
	{

	}

	public override void _Ready()
	{
		CardDatas.Clear();
		for (int i = 0; i < cardValuesPaths.Count; i++)
		{
			int value = cardValuesPaths[i].Item1;
			Texture2D text = GD.Load<Texture2D>(cardValuesPaths[i].Item2);
			CardDatas.Add((value, text));
		}

		setupNodesFromScene();
		setupCellNodes();
		createPlayers();
		startGame();
	}


	private void setupNodesFromScene()
	{
		_startingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");
		testbutton = GetNode<Button>("Button");


	}


	private void setupCellNodes()
	{
		////// CELLÁK
		HBoxContainer hbox = GetNode<HBoxContainer>("Center/HBoxContainer1");
		foreach (Cell child in hbox.GetChildren())
		{
			_cells.Add(child);
		}
		hbox = GetNode<HBoxContainer>("Center/HBoxContainer2");
		foreach (Cell child in hbox.GetChildren())
		{
			_cells.Add(child);
		}

	}

	private void createPlayers()
	{
		CardContainer container0 = GetNode<CardContainer>("PLAYER/HBoxContainer");
		CardContainer container1 = GetNode<CardContainer>("BOT1");
		CardContainer container2 = GetNode<CardContainer>("BOT2");
		CardContainer container3 = GetNode<CardContainer>("BOT3");
		Cell gameArea = GetNode<Cell>("Center/HBoxContainer2/GameArea");

		_player = new Player("player", 0, _score, container0, gameArea);

		_bot1 = new Bot("bot1", 1, _score, container1, gameArea);
		_bot2 = new Bot("bot2", 2, _score, container2, gameArea);
		_bot3 = new Bot("bot3", 3, _score, container3, gameArea);
		_bots = new List<Bot> { _bot1, _bot2, _bot3 };
		_allPlayers = new List<CardHolderBase> { _player, _bot1, _bot2, _bot3 };

	}
	private void OnPlayerCardClicked(PlayerCard card)
	{

		if (_startingCardValue == -1)
		{
			_player.startRound(_cells, ref _startingCardValue, card);
			_startingValueLabel.setText(ref _startingCardValue);
			cardsInArea.Add(card);
			botsRounds();

			return;
		}
		
		else
		{
			cardsInArea.Add(card);
			_player.normalRound(_cells, _startingCardValue, card);
			botsRounds();
		}
	}
	public void onNewRoundButtonPressed()
	{
		ToggleNewRoundButton(false);
		toggleExitButton(false);
		VBoxContainer center = GetNode<VBoxContainer>("Center");
		VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");
		center.Show();
		statCenter.Hide();
		startGame();
	}


	private async void onWin(int winnerid)
	{

		int sumPoint = 0;
		for (int i = 0; i < 4; i++)
		{
			if (winnerid == i) continue;
			sumPoint += _allPlayers[i].getCardsInHandCount();
			_allPlayers[i].onLose();

		}
		_allPlayers[winnerid].onWin();

		if (isOver())
		{
			GD.Print("GAME OVER");
			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
			RichTextLabel winLabel = GetNode<RichTextLabel>("StatCenter/WinLabel");
			VBoxContainer center = GetNode<VBoxContainer>("Center");
			VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");

			center.Hide();
			statCenter.Show();
			var info = whoWon();
			CardHolderBase winner = info.Item1;
			int position = info.Item2;
			winLabel.Text = winner.getName() + " NYERT!\n\n";
			winLabel.Text += "GRATULÁLUNK! \n" + position + ". LETTÉL!";
			toggleExitButton(true);

		}
		ToggleNewRoundButton(true);
		toggleExitButton(true);


	}
	private (CardHolderBase, int) whoWon()
	{
		int playerScore = _allPlayers[0].getScore();
		int position = 1;
		int max = _allPlayers[0].getScore();
		int index = 0;

		for (int i = 1; i < _allPlayers.Count; i++)
		{
			if (max < _allPlayers[i].getScore())
			{
				max = _allPlayers[i].getScore();
				index = i;
			}
			if (playerScore < _allPlayers[i].getScore()) position++;
		}
		return (_allPlayers[index], position);
	}
	private bool isOver()
	{

		return false;
	}
	private void ToggleNewRoundButton(bool enabled)
	{
		Button newRoundButton = GetNode<Button>("ButtonsContainer/NewRoundButton");
		newRoundButton.Visible = enabled;
	}
	private void toggleExitButton(bool enabled)
	{
		Button exitButton = GetNode<Button>("ButtonsContainer/ExitButton");
		exitButton.Visible = enabled;
	}

	private void roundEnd()
	{
		CardHolderBase roundWinner = null;
		GD.Print("WHO STARTED: " + _whoStarted);
		var starterCardValue = cardsInArea.First().getValue();
		for (int i = 0; i < cardsInArea.Count; i++)
		{
			CardBase card = cardsInArea[i];
			if (isSameType(card.getValue(), starterCardValue) || isVII(card.getValue()))
			{
				GD.Print("kezdolap: " + starterCardValue + " megfelel: " + card.getValue());
				GD.Print("WHO STARTED SZAMOLAS: " + (_whoStarted + i) % 4);
				roundWinner = _allPlayers[(_whoStarted + i) % 4];
			}
		}
		roundWinner.takenCards.AddRange(cardsInArea);
		cardsInArea.Clear();
		GD.Print("NYERTES: " + roundWinner.getName());
		roundNew(roundWinner);

	}
	private void roundNew(CardHolderBase starts)
	{
		_startingValueLabel.removeText();
		_startingCardValue = -1;

		foreach (Cell item in _cells)
		{
			item.resetCell();
		}

		foreach (var p in _allPlayers)
		{
			p.newRoundDeal();
		}

		foreach (PlayerCard item in _player.getList())
		{

			item.CardClicked -= OnPlayerCardClicked;
			item.CardClicked += OnPlayerCardClicked;
		}
		_player.disableCards();
		_whoStarted = starts.Id;
		if (starts is Player player)
		{
			GD.Print("player kezd");
			player.enableCards();
		}
		else if (starts is Bot bot)
		{

			GD.Print(bot.Name + " kezd");
			CardBase selectedCard = bot.startRound(ref _startingCardValue);
			cardsInArea.Add(selectedCard);
			_startingValueLabel.setText(ref _startingCardValue);
			botsRounds(bot.Id - 1);
		}

	}

	private bool isSameType(int value, int startingCardValue)
	{
		return (value % 10) == (startingCardValue % 10);

	}
	private bool isVII(int value)
	{
		if (value % 10 == 5) return true;
		else return false;
	}


	private async void botsRounds(int fromWho = -1)
	{
		fromWho++;
		while (fromWho < 3)
		{
			await ToSignal(GetTree().CreateTimer(1.4f), "timeout");
			if (StartingPlayer.Equals(_bots[fromWho]))
			{
				roundEnd();
				return;
			}
			GD.Print(fromWho + 1 + ". bot");

			cardsInArea.Add(_bots[fromWho].normalRound(_startingCardValue));
			fromWho++;

		}
		await ToSignal(GetTree().CreateTimer(1.2f), "timeout");
		if(StartingPlayer.Equals(_player))
			roundEnd();
		else playerRound();
	}
	private void playerRound()
	{
		GD.Print("player jon");

		_player.enableCards();
	}



	private async void startGame()
	{


		_startingValueLabel.removeText();
		_startingCardValue = -1;
		foreach (Cell item in _cells)
		{
			item.resetCell();
		}


		foreach (CardHolderBase player in _allPlayers)
		{
			player.resetState();
			player.deal();
		}
		_player.disableCards();

		foreach (PlayerCard item in _player.getList())
		{
			GD.Print("siker");
			item.CardClicked += OnPlayerCardClicked;
		}


		Random random = new Random();
		_whoStarted = random.Next(0, 4);
		_whoStarted=0;


		await ToSignal(GetTree().CreateTimer(1f), "timeout");

		if (_whoStarted == 0)
		{
			GD.Print("player kezd");
			_player.enableCards();
		}
		else
		{
			GD.Print(_whoStarted + ". bot kezd");
			CardBase selectedCard = _bots[_whoStarted - 1].startRound(ref _startingCardValue);
			cardsInArea.Add(selectedCard);
			_startingValueLabel.setText(ref _startingCardValue);
			botsRounds(_whoStarted - 1);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void onTestButtonPressed()
	{
		GD.Print("TEST PRESSED");

		QueueFree();

	}

	public void onExitButtonPressed()
	{
		GD.Print("EXIT");
		toggleExitButton(false);
		GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");
		//QueueFree();
	}
}
