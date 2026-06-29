using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace zsir;

public partial class Zsir : Control
{
    private List<Cell> _cells = new List<Cell>();
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

    public override void _Ready()
    {
        CardDatabase.loadTextures();
        setupNodesFromScene();
        setupCellNodes();
        createPlayers();
        StartGame();
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

        var player = new Player("player", 0, container0, gameArea);
        var bots = new List<Bot>
        {
            new Bot("bot1", 1, container1, gameArea),
            new Bot("bot2", 2, container2, gameArea),
            new Bot("bot3", 3, container3, gameArea)
        };

        _gameLogic.SetupPlayers(player, bots);
    }

    private void OnPlayerCardClicked(PlayerCard card)
    {
        int startingValue = _gameLogic.StartingCardValue;
        _gameLogic.CardsInArea.Add(card);
        if (startingValue == -1)
        {
            _gameLogic.HumanPlayer.StartRound(_cells, ref startingValue, card);
            _gameLogic.StartingCardValue = startingValue;
            _startingValueLabel.setText(ref startingValue);
        }
        else
        {
            _gameLogic.HumanPlayer.NormalRound(_cells, startingValue, card);
        }

        BotsRounds(0);
    }

    public void onNewRoundButtonPressed()
    {
        ToggleNewRoundButton(false);
        ToggleExitButton(false);
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");
        center.Show();
        statCenter.Hide();
        StartGame();
    }
    
    private void RoundEnd()
    {
        EntityBase roundWinner = _gameLogic.EvaluateRoundWinner();
        GD.Print("kör nyertes: " + roundWinner.Name);
        if (_gameLogic.IsGameOver())
        {
            GameOver();
        }
        else RoundNew(roundWinner);
    }

    private async void GameOver()
    {
        GD.Print("GAME OVER");
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        RichTextLabel winLabel = GetNode<RichTextLabel>("StatCenter/WinLabel");
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");

        center.Hide();
        statCenter.Show();
        var (winner, position) = _gameLogic.GetGameResult();
        winLabel.Text = $"{winner.Name} NYERT!\n\nGRATULÁLUNK! \n{position}. LETTÉL!";
        ToggleNewRoundButton(true);
        ToggleExitButton(true);
    }

    private void RoundNew(EntityBase starterEntity)
    {
        _gameLogic.ResetRoundState();
        _startingValueLabel.removeText();


        foreach (Cell item in _cells)
        {
            item.resetCell();
        }

        foreach (var p in _gameLogic.AllPlayers)
        {
            p.NewRoundDeal(_gameLogic);
        }

        foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
        {
            item.CardClicked -= OnPlayerCardClicked;
            item.CardClicked += OnPlayerCardClicked;
        }

        _gameLogic.HumanPlayer.DisableCards();
        _gameLogic.WhoStarted = starterEntity.Id;
        if (starterEntity is Player player)
        {
            GD.Print("player kezd");
            player.EnableCards();
        }
        else if (starterEntity is Bot bot)
        {
            int startingValue = _gameLogic.StartingCardValue;
            GD.Print(bot.Name + " kezd");
            CardBase selectedCard = bot.StartRound(ref startingValue);
            _gameLogic.StartingCardValue = startingValue;
            _gameLogic.CardsInArea.Add(selectedCard);
            _startingValueLabel.setText(ref startingValue);
            BotsRounds(bot.Id);
        }
    }


    private async Task BotsRounds(int fromWho)
    {
        int botIdx = fromWho;
        
        while (botIdx < 3)
        {
            await ToSignal(GetTree().CreateTimer(1.4f), "timeout");
            if (_gameLogic.StartingPlayer.Equals(_gameLogic.Bots[botIdx]))
            {
                var card = _gameLogic.Bots[botIdx].NormalRound(_gameLogic.StartingCardValue, false);
                if (card is null) RoundEnd();
                else
                {
                    _gameLogic.CardsInArea.Add(card);
                    BotsRounds(_gameLogic.Bots[botIdx].Id);
                }
                return;
            }

            GD.Print(botIdx + 1 + ". bot");

            var botCard = _gameLogic.Bots[botIdx].NormalRound(_gameLogic.StartingCardValue,true);
            _gameLogic.CardsInArea.Add(botCard);
            botIdx++;
        }

        await ToSignal(GetTree().CreateTimer(1.2f), "timeout");
        if (_gameLogic.StartingPlayer.Equals(_gameLogic.HumanPlayer) && !_gameLogic.HumanPlayer.IsHavePlayableCard(_gameLogic.StartingCardValue))
        {
           RoundEnd();
        }
        else PlayerRound();
    }

    private void PlayerRound()
    {
        GD.Print("player jon");
        if(_gameLogic.CardsInArea.Count != 0 && _gameLogic.StartingPlayer.Equals(_gameLogic.HumanPlayer))
            _passButton.Visible = true;
        
        
        _gameLogic.HumanPlayer.EnableCards();
    }


    private async void StartGame()
    {
        _startingValueLabel.removeText();
   
        _gameLogic.ResetGameState();
        foreach (Cell item in _cells)
        {
            item.resetCell();
        }


        foreach (EntityBase player in _gameLogic.AllPlayers)
        {
            player.ResetState();
            player.NewGameDeal(_gameLogic);
        }

        _gameLogic.HumanPlayer.DisableCards();

        foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
        {
            item.CardClicked += OnPlayerCardClicked;
        }

        Random random = new Random();
        _gameLogic.WhoStarted = random.Next(0, 4);
        int whoStarted = _gameLogic.WhoStarted;


        await ToSignal(GetTree().CreateTimer(1f), "timeout");

        if (whoStarted == 0)
        {
            GD.Print("player kezd");
            _gameLogic.HumanPlayer.EnableCards();
        }
        else
        {
            int startingValue = _gameLogic.StartingCardValue;
            GD.Print(whoStarted + ". bot kezd");
            CardBase selectedCard = _gameLogic.Bots[whoStarted - 1].StartRound(ref startingValue);
            _gameLogic.CardsInArea.Add(selectedCard);
            _startingValueLabel.setText(ref startingValue);
            _gameLogic.StartingCardValue = startingValue;
            BotsRounds(whoStarted);
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

    private void ToggleNewRoundButton(bool enabled) =>
        GetNode<Button>("ButtonsContainer/NewRoundButton").Visible = enabled;

    private void ToggleExitButton(bool enabled) => GetNode<Button>("ButtonsContainer/ExitButton").Visible = enabled;
    private void OnExitButtonPressed() => GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");

    private void onPassButtonPressed()
    {
        _passButton.Visible = false;
        RoundEnd();
    }
}