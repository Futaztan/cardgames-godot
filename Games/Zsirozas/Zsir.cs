using System.Collections.Generic;
using System.Threading.Tasks;
using cardgames.Zsirozas.Players;
using Godot;
using zsir;

namespace cardgames.Zsirozas;

public partial class Zsir : Control
{
	public static Cell GameAreaCell;
	private Cell _cardDeckCell = new();
	private StartingCardLabel _startingValueLabel;
	private Button _passButton;

	private ZsirGameLogic _gameLogic;


	//TODO diflabelek utan elbaszodik a label meret és nagyobb lesz,  ??? nem talaltam meg megint ezt
	//TODO: LORUMBA A BEKÖTÉSEK SZAROK NAGY BETU MIATT VALSZEG

	/* 1. zold
	 *  2. piros
	 * 3. makk
	 * 4. tok
	 *
	 */


	//ha roundsuntilend = 0, majd -1 akkor amíg el nem fogynak a pontok
	public void Init()
	{
	}

	public async override void _Ready()
	{
		CardDatabase.loadTextures();
		SetupNodesFromScene();
		SetupCellNodes();
		CreatePlayers();
		_gameLogic.OnRoundStarted += OnLogicRoundStarted;
		//_gameLogic.OnCardPlayed += OnLogicCardPlayed;
		_gameLogic.OnRoundEnded += OnLogicRoundEnded;
		_gameLogic.OnGameOver += OnLogicGameOver;
		_gameLogic.OnPlayerTurnStarted += OnLogicPlayerTurnStarted;
		_gameLogic.OnReset += OnLogicReset;
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		_gameLogic.StartNewGame();
	}

	private async Task OnLogicRoundEnded(EntityBase entity)
	{
		Control toNode = entity.CardContainer;
		TextureRect animatedCard = new TextureRect();
		animatedCard.Texture = GameAreaCell.GetTexture();
		animatedCard.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		animatedCard.StretchMode = TextureRect.StretchModeEnum.KeepAspect;
		animatedCard.Size = 1.15f * GameAreaCell.Size;
		this.AddChild(animatedCard);
		animatedCard.GlobalPosition = GameAreaCell.GlobalPosition;
		animatedCard.PivotOffset = animatedCard.Size / 2.0f;
        GameAreaCell.resetCell();
		Tween tween = this.CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(animatedCard, "global_position", toNode.GlobalPosition + new Vector2(toNode.Size.X/2f,0) - new Vector2(animatedCard.Size.X/2f,0), 0.5f);
		await ToSignal(tween, Tween.SignalName.Finished);
		animatedCard.QueueFree();
	}

	private void SetupNodesFromScene()
	{
		_startingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");
		_passButton = GetNode<Button>("Center/HBoxContainer2/GameArea/PassButton");
	}

	private void SetupCellNodes()
	{
		////// CELLÁK
		_cardDeckCell = GetNode<Cell>("Center/HBoxContainer1/CardDeck");
		GameAreaCell = GetNode<Cell>("Center/HBoxContainer2/GameArea");
	}

	private void CreatePlayers()
	{
		CardContainer container0 = GetNode<CardContainer>("PLAYER/HBoxContainer");
		CardContainer container1 = GetNode<CardContainer>("BOT1");
		CardContainer container2 = GetNode<CardContainer>("BOT2");
		CardContainer container3 = GetNode<CardContainer>("BOT3");


		var player = new Player("player", 0, container0);
		var bots = new List<Bot>
		{
			new Bot("bot1", 1, container1),
			new Bot("bot2", 2, container2),
			new Bot("bot3", 3, container3)
		};
		_gameLogic = new ZsirGameLogic(player, bots);
	}

	private void OnPlayerCardClicked(PlayerCard card)
	{
		_passButton.Visible = false;
		_gameLogic.HumanPlayer.DisableCards();
		_gameLogic.PlayHumanCard(card);
	}

	public void OnNewRoundButtonPressed()
	{
		ToggleNewRoundButton(false);
		ToggleExitButton(false);
		VBoxContainer center = GetNode<VBoxContainer>("Center");
		var gameResults = GetNode<Control>("GameResults");
		center.Show();
		gameResults.Hide();
		_gameLogic.StartNewGame();
	}


	private void OnLogicGameOver(List<EntityBase> entities)
	{
		GD.Print("GAME OVER");
		VBoxContainer center = GetNode<VBoxContainer>("Center");
		Control gameResults = GetNode<Control>("GameResults");

		for (int i = 0; i < entities.Count; i++)
		{
			AddRowToGrid(i+1, entities[i].Name, entities[i].Score);
		}
		center.Hide();
		gameResults.Show();
		ToggleNewRoundButton(true);
		ToggleExitButton(true);
	}

	private void AddRowToGrid(int position, string name, int score)
	{
		GridContainer grid = GetNode<GridContainer>("GameResults/StatCenter/GridContainer");
		Font font = GD.Load<Font>("res://Assets/Fonts/Montserrat-Regular.ttf");
		Theme theme = new Theme { DefaultFont = font, DefaultFontSize = 32 };
		grid.AddChild(new Label { Text = position + ".", Theme = theme, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center});
		grid.AddChild(new Label { Text = name, Theme = theme, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center});
		grid.AddChild(new Label { Text = score.ToString(), Theme = theme, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center});
	}

	private void OnLogicPlayerTurnStarted()
	{
		GD.Print("Player jön");
		foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
		{
			item.CardClicked -= OnPlayerCardClicked;
			item.CardClicked += OnPlayerCardClicked;
		}

		if (_gameLogic.CardsInArea.Count != 0 && _gameLogic.StartingPlayer.Equals(_gameLogic.HumanPlayer))
		{
			_passButton.Visible = true;
			_gameLogic.HumanPlayer.EnablePlayableCards();
		}
		else _gameLogic.HumanPlayer.EnableAllCards();
	}


	private void OnLogicRoundStarted(EntityBase startinPlayer, int startingValue)
	{
		if (startingValue == -1)
			_startingValueLabel.removeText();
		else
			_startingValueLabel.setText(startinPlayer, startingValue);
	}

	private async void OnLogicReset()
	{
		_startingValueLabel.removeText();
		GameAreaCell.resetCell();
		_cardDeckCell.resetCell();
		_gameLogic.HumanPlayer.DisableCards();

		/* foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
		 {
			 item.CardClicked += OnPlayerCardClicked;
		 }*/


		await ToSignal(GetTree().CreateTimer(1f), "timeout");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}




	private void ToggleNewRoundButton(bool enabled) =>
		GetNode<Button>("ButtonsContainer/NewRoundButton").Visible = enabled;

	private void ToggleExitButton(bool enabled) => GetNode<Button>("ButtonsContainer/ExitButton").Visible = enabled;
	private void OnExitButtonPressed() => GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");

	private void OnPassButtonPressed()
	{
		_passButton.Visible = false;
		_gameLogic.RoundEnd();
	}
}
