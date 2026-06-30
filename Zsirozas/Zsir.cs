using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace zsir;

public partial class Zsir : Control
{
    public static Cell GameAreaCell;
    private Cell _cardDeckCell = new();
    private StartingCardLabel _startingValueLabel;
    private Button testbutton;
    private Button _passButton;

    private ZsirGameLogic _gameLogic = new();


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

    public async override void _Ready()
    {
        CardDatabase.loadTextures();
        setupNodesFromScene();
        setupCellNodes();
        createPlayers();
        _gameLogic.OnRoundStarted += OnLogicRoundStarted;
        //_gameLogic.OnCardPlayed += OnLogicCardPlayed;
        //_gameLogic.OnRoundEnded += OnLogicRoundEnded;
        _gameLogic.OnGameOver += OnLogicGameOver;
        _gameLogic.OnPlayerTurnStarted += OnLogicPlayerTurnStarted;
        _gameLogic.OnReset += OnLogicReset;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _gameLogic.StartNewGame();
    }
    
    private void setupNodesFromScene()
    {
        _startingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");
        testbutton = GetNode<Button>("Button");
        _passButton = GetNode<Button>("PassButton");
    }

    private void setupCellNodes()
    {
        ////// CELLÁK
        _cardDeckCell = GetNode<Cell>("Center/HBoxContainer1/CardDeck");
        GameAreaCell = GetNode<Cell>("Center/HBoxContainer2/GameArea");
    }

    private void createPlayers()
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

        _gameLogic.SetupPlayers(player, bots);
    }

    private void OnPlayerCardClicked(PlayerCard card)
    {
        _passButton.Visible = false;
        _gameLogic.HumanPlayer.DisableCards();
        _gameLogic.PlayHumanCard(card);
    }

    public void onNewRoundButtonPressed()
    {
        ToggleNewRoundButton(false);
        ToggleExitButton(false);
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");
        center.Show();
        statCenter.Hide();
        _gameLogic.StartNewGame();
    }


    private async void OnLogicGameOver(EntityBase winner, int position)
    {
        GD.Print("GAME OVER");
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        RichTextLabel winLabel = GetNode<RichTextLabel>("StatCenter/WinLabel");
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");

        center.Hide();
        statCenter.Show();

        winLabel.Text = $"{winner.Name} NYERT!\n\nGRATULÁLUNK! \n{position}. LETTÉL!";
        ToggleNewRoundButton(true);
        ToggleExitButton(true);
    }

    private void OnLogicPlayerTurnStarted()
    {
        GD.Print("Player jön");
        _gameLogic.HumanPlayer.EnableCards();

        foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
        {
            item.CardClicked -= OnPlayerCardClicked;
            item.CardClicked += OnPlayerCardClicked;
        }

        if (_gameLogic.CardsInArea.Count != 0 && _gameLogic.StartingPlayer.Equals(_gameLogic.HumanPlayer))
        {
            _passButton.Visible = true;
        }
    }


    private void OnLogicRoundStarted(int startingValue)
    {
        if (startingValue == -1)
            _startingValueLabel.removeText();
        else
            _startingValueLabel.setText(startingValue);
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


    public void onTestButtonPressed()
    {
        GD.Print("TEST PRESSED");

        QueueFree();
    }

    private void ToggleNewRoundButton(bool enabled) =>
        GetNode<Button>("ButtonsContainer/NewRoundButton").Visible = enabled;

    private void ToggleExitButton(bool enabled) => GetNode<Button>("ButtonsContainer/ExitButton").Visible = enabled;
    private void OnExitButtonPressed() => GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");

    private void onPassButtonPressed()
    {
        _passButton.Visible = false;
        _gameLogic.RoundEnd();
    }
}