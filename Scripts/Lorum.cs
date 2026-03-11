using Godot;
using System;
using System.Collections.Generic;

public partial class Lorum : Control
{
	private Bot _bot1;
	private Bot _bot2;
	private Bot _bot3;
	private Player _player;
	private List<Bot> bots;
	private int startingCardValue; //maradékos osztás 10
	private List<Cell> cells = new List<Cell>();
	private StartingCardLabel startingValueLabel;
	public static Pass passIcon;

	private Button testbutton;

	private List<CardHolderBase> _allPlayers;
	private int whoStarted = -1;
	private int _score;
	private int _roundsUntilEnd;


	private List<(int, Texture2D)> cardDatas = new List<(int, Texture2D)>();

	private List<(int, string)> tmp = new List<(int, string)>
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




	private PackedScene _pointLabelScene;
	private List<RichTextLabel> _pointLabels = new List<RichTextLabel>();

	//TODO diflabelek utan elbaszodik a label meret és nagyobb lesz, 
	// rendezni a kapott kartyakat, 
	// default value a settings popupnak
	//	startcardlabel reset uj kornel

	/* 1. zold
	*  2. piros
	* 3. makk
	* 4. tok
	*
	*/


	//ha roundsuntilend = 0, majd -1 akkor amíg el nem fogynak a pontok
	public void init(int score, int roundsUntilEnd)
	{
		_score = score;
		_roundsUntilEnd = roundsUntilEnd;
		if (_roundsUntilEnd == 0) _roundsUntilEnd--;
	}

	public override void _Ready()
	{

		for (int i = 0; i < tmp.Count; i++)
		{
			int value = tmp[i].Item1;
			Texture2D text = GD.Load<Texture2D>(tmp[i].Item2);
			cardDatas.Add((value, text));
		}



		setupNodesFromScene();
		setupCellNodes();
		createPlayers();
		startGame();


	}


	private void setupNodesFromScene()
	{
		startingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");
		testbutton = GetNode<Button>("Button");

		passIcon = GetNode<Pass>("PassIcon");
		passIcon.PivotOffset = passIcon.Size * 0.5f;
		_pointLabelScene = (PackedScene)GD.Load("res://Scenes/PointLabel.tscn");

		for (int i = 0; i < 4; i++)
		{
			RichTextLabel pointLabel = (RichTextLabel)_pointLabelScene.Instantiate();
			_pointLabels.Add(pointLabel);
			this.AddChild(pointLabel);
		}
		Container box = GetNode<Container>("PLAYER");
		_pointLabels[0].Text = "[b]" + "PLAYER" + "\n" + _score + " pont [/b]";
		_pointLabels[0].Size = _pointLabels[0].GetMinimumSize();
		_pointLabels[0].SetPosition(new Vector2(0, box.GlobalPosition.Y - _pointLabels[0].Size.Y - _pointLabels[0].Size.Y * 0.5F));

		box = GetNode<Container>("BOT1");
		_pointLabels[1].Text = "[b]" + "BOT1" + "\n" + _score + " pont [/b]";
		_pointLabels[1].Size = _pointLabels[1].GetMinimumSize();
		_pointLabels[1].SetPosition(new Vector2(0, box.GlobalPosition.Y - _pointLabels[1].Size.Y));

		box = GetNode<Container>("BOT2");
		_pointLabels[2].Text = "[b]" + "BOT2" + "\n" + _score + " pont [/b]";
		_pointLabels[2].Size = _pointLabels[2].GetMinimumSize();
		_pointLabels[2].SetPosition(new Vector2(box.Size.X * 0.5f - _pointLabels[2].Size.X * 0.5f, box.Size.Y - Mathf.Abs(box.GlobalPosition.Y)));

		box = GetNode<Container>("BOT3");
		_pointLabels[3].Text = "[b]" + "BOT3" + "\n" + _score + " pont [/b]";
		_pointLabels[3].Size = _pointLabels[3].GetMinimumSize();
		Vector2 size = GetViewport().GetVisibleRect().Size;

		_pointLabels[3].SetPosition(new Vector2(size.X - _pointLabels[3].Size.X, box.GlobalPosition.Y - _pointLabels[3].Size.Y));


	}


	private void setupCellNodes()
	{
		////// CELLÁK
		HBoxContainer hbox = GetNode<HBoxContainer>("Center/HBoxContainer1");
		foreach (Cell child in hbox.GetChildren())
		{
			cells.Add(child);
		}
		hbox = GetNode<HBoxContainer>("Center/HBoxContainer2");
		foreach (Cell child in hbox.GetChildren())
		{
			cells.Add(child);
		}

	}




	private void createPlayers()
	{
		CardContainer container0 = GetNode<CardContainer>("PLAYER/HBoxContainer");
		CardContainer container1 = GetNode<CardContainer>("BOT1");
		CardContainer container2 = GetNode<CardContainer>("BOT2");
		CardContainer container3 = GetNode<CardContainer>("BOT3");

		_player = new Player("player", _score, _pointLabels[0], container0);
		_player.disableCards();
		_bot1 = new Bot("bot1", _score, _pointLabels[1], container1);
		_bot2 = new Bot("bot2", _score, _pointLabels[2], container2);
		_bot3 = new Bot("bot3", _score, _pointLabels[3], container3);
		bots = new List<Bot> { _bot1, _bot2, _bot3 };
		_allPlayers = new List<CardHolderBase> { _player, _bot1, _bot2, _bot3 };

	}
	private void OnPlayerCardClicked(PlayerCard card)
	{
		if (startingCardValue == -1)
		{
			_player.startRound(cells, ref startingCardValue, card);
			startingValueLabel.setText(ref startingCardValue);

			botsRounds();
			return;
		}
		int cardCount = _player.normalRound(cells, startingCardValue, card);
		if (cardCount >= 0)
		{
			if (cardCount == 0)
			{
				onWin(0);
			}
			else botsRounds();
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
		foreach (CardHolderBase item in _allPlayers)
		{
			item.UpdateLabel();
		}
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
		_allPlayers[winnerid].onWin(sumPoint);

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
		
		if (_roundsUntilEnd == -1)
		{
			foreach (CardHolderBase item in _allPlayers)
			{
				if (item.getScore() <= 0)
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			_roundsUntilEnd--;
			if (_roundsUntilEnd == 0)
			{
				return true;
			}
			return false;
		}
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


	private async void botsRounds(int fromWho = -1)
	{
		fromWho++;
		while (fromWho < 3)
		{
			await ToSignal(GetTree().CreateTimer(1.4f), "timeout");
			GD.Print(fromWho + 1 + ". bot");
			if (bots[fromWho].normalRound(cells, startingCardValue) == 0)
			{
				onWin(fromWho + 1);
				return;
			}
			fromWho++;

		}
		await ToSignal(GetTree().CreateTimer(1.2f), "timeout");
		playerRound();
	}
	private void playerRound()
	{
		GD.Print("player jon");
		if (!_player.canPlaceCard(cells, startingCardValue))
		{
			GD.Print("Player passz");
			botsRounds();
		}
		else _player.enableCards();
	}



	private async void startGame()
	{

		List<int> usedRandoms = new List<int>();
		startingCardValue = -1;
		foreach (Cell item in cells)
		{
			item.resetCell();
		}

		foreach (CardHolderBase player in _allPlayers)
		{
			player.resetState();
			player.deal(usedRandoms, cardDatas);
		}
		_player.disableCards();

		foreach (PlayerCard item in _player.getList())
		{
			GD.Print("siker");
			item.CardClicked += OnPlayerCardClicked;
		}

		int whoStarts;
		if (whoStarted == -1)
		{
			Random random = new Random();
			whoStarts = random.Next(0, 4);
		}
		else
		{
			whoStarts = whoStarted + 1;
			if (whoStarts == 4) whoStarts = 0;
		}
		whoStarted = whoStarts;

		await ToSignal(GetTree().CreateTimer(1f), "timeout");

		if (whoStarts == 0)
		{
			GD.Print("player kezd");
			_player.enableCards();
		}
		else
		{
			GD.Print(whoStarts + ". bot kezd");
			bots[whoStarts - 1].startRound(cells, ref startingCardValue);
			startingValueLabel.setText(ref startingCardValue);
			botsRounds(whoStarts - 1);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void onTestButtonPressed()
	{
		GD.Print("TEST PRESSED");



	}

	public void onExitButtonPressed()
	{
		GD.Print("EXIT");
		toggleExitButton(false);
		//TODO
	}
}
