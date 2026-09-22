using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cardgames.Games._21.Scripts;
using cardgames.Games._21.Scripts.Cards;
using cardgames.Games._21.Scripts.Players;


public partial class _21 : Control
{
    [Export] private Cell CardDeckCell { get; set; }
    [Export] private PackedScene MainMenuScene {get; set;}

    private _21GameLogic _gameLogic;


    public override async void _Ready()
    {
        CardDatabase.loadTextures();


        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        CreatePlayers();
        _gameLogic.OnWagerChanged += OnLogicWagerChanged;
        _gameLogic.OnSelectWager += OnLogicSelectWager;
        _gameLogic.OnAskPlayerForMove += OnLogicAskPlayerForMove;
        _gameLogic.OnRoundOver += OnLogicRoundOver;
        _gameLogic.OnReset += OnLogicResetRound;
        _gameLogic.OnContinueGameAfterDeal += OnLogicContinueGameAfterDeal;
        _gameLogic.OnStartBotRound += OnLogicStartBotRound;
        _gameLogic.OnGameOver += OnLogicGameOver;
        _gameLogic.StartNewGame();
    }

    private void OnLogicGameOver(EntityBase winner)
    {
        GetNode<ColorRect>("%GameOverMenu").Visible = true;
        var whowon = GetNode<Label>("Player/GameOverMenu/PanelContainer/VBoxContainer/Label1");
        var desc = GetNode<Label>("Player/GameOverMenu/PanelContainer/VBoxContainer/Label2");
        whowon.Text = winner.Name + " nyert!";
        if (winner is Player)
        {
            desc.Text = "Elfogytak az osztó pontjai, így Te nyertél!\nIndulhat a következő kör?";
        }
        else desc.Text = "Elfogytak a pontjaid, így az osztó nyert.\nIndulhat a következő kör?";
    }

    private void OnLogicResetRound()
    {
        ToggleDecisionMenuButtonsVisibility(true);
        ToggleDecisionMenuButtonsClick(true);
        GetNode<Container>("%NewRoundContainer").Visible = false;
        GetNode<Label>("Bot/ValueLabel").Visible = false;
        ToggleDecisionMenuVisibility(false);
    }

    private void CreatePlayers()
    {
        CardContainer container0 = GetNode<CardContainer>("%PlayerContainer");
        CardContainer container1 = GetNode<CardContainer>("%BotContainer");
        Label label0 = GetNode<Label>("%PlayerPointLabel");
        Label label1 = GetNode<Label>("%BotPointLabel");
        Label valueLabel0 = GetNode<Label>("Player/DecisionMenu/ValueLabel");
        Label valueLabel1 = GetNode<Label>("Bot/ValueLabel");
        var goldcoin0 = GetNode<GoldCoin>("%PlayerGoldCoin");
        var goldcoin1 = GetNode<GoldCoin>("%BotGoldCoin");
        int money = 100;

        var player = new Player("player", 0, container0, money, label0, valueLabel0, goldcoin0);
        var bot = new Bot("bot", 1, container1, money, label1,valueLabel1, goldcoin1);
        _gameLogic = new _21GameLogic(player, bot, money);
    }

    private void OnModifyWagerButtonPressed(long value)
    {
        _gameLogic.Wager += (int)value;
    }

    private void OnLogicWagerChanged(int wager)
    {
        Label wagerLabel = GetNode<Label>("%WagerLabel");
        wagerLabel.Text = wager + "$";
    }

    private void OnLogicSelectWager(int wager, int maxWager)
    {
        ColorRect menu = GetNode<ColorRect>("%WagerMenu");
        menu.Visible = true;
        Label maxLabel = GetNode<Label>("%MaxWagerLabel");
        maxLabel.Text = "MAXIMÁLIS TÉT: " + maxWager; 
        OnLogicWagerChanged(wager);
    }

    private void OnLogicAskPlayerForMove(int currentPlayerScore)
    {
        ToggleDecisionMenuVisibility(true);
        ToggleStopButtonDisable();
        //OnPlayerValueChanged();
    }

    private void ToggleDecisionMenuVisibility(bool isVisible)
    {
        var menu = GetNode<Container>("%DecisionMenu");
        menu.Visible = isVisible;
    }


    private void OnWagerOkButtonPressed()
    {
        ColorRect menu = GetNode<ColorRect>("%WagerMenu");
        menu.Visible = false;
        _gameLogic.SecondDeal();
    }

    private async void OnAskingForCardButtonPressed()
    {
        ToggleDecisionMenuButtonsClick(false);
        await _gameLogic.NDeal();
        //OnPlayerValueChanged();
    }
    

    private void OnLogicRoundOver()
    {
         
     
        GetNode<Label>("Bot/ValueLabel").Visible = true;
        ToggleDecisionMenuButtonsVisibility(false);
        ToggleDecisionMenuVisibility(true);
        GetNode<Container>("%NewRoundContainer").Visible = true;
    }
    

    private void OnNewRoundButtonPressed()
    {
        _gameLogic.StartNewRound();
    }

    private void OnLogicContinueGameAfterDeal()
    {
        ToggleStopButtonDisable();
        ToggleDecisionMenuButtonsClick(true);
    }

    private void ToggleDecisionMenuButtonsClick(bool isClickable)
    {
        var menu = GetNode<HBoxContainer>("%DecisionMenu/HBoxContainer");

        if (isClickable)
        {
            foreach (var button in menu.GetChildren().OfType<Button>())
            {
                button.MouseFilter = MouseFilterEnum.Pass;
            }
        }
        else
        {
            foreach (var button in menu.GetChildren().OfType<Button>())
            {
                button.MouseFilter = MouseFilterEnum.Ignore;
            }
        }
    }

    private void ToggleDecisionMenuButtonsVisibility(bool isVisible)
    {
        var menu = GetNode<HBoxContainer>("%DecisionMenu/HBoxContainer");
        foreach (var button in menu.GetChildren().OfType<Button>())
        {
            button.Visible = isVisible;
        }
    }

    private void OnPlayerValueChanged()
    {
        var label = GetNode<Label>("%DecisionMenu/ValueLabel");
        if (_gameLogic.PlayerCardsValue > 21)
        {
            label.AddThemeColorOverride("font_color", new Color("#ff9999"));
        }
        else label.AddThemeColorOverride("font_color", new Color(Colors.White));

        label.Text = "Érték: " + _gameLogic.PlayerCardsValue;
    }

    private void ToggleStopButtonDisable()
    {
        var stopButton = GetNode<Button>("%DecisionMenu/HBoxContainer/StopButton");
        stopButton.Disabled = (_gameLogic.PlayerCardsValue < 15);
    }

    private void OnLogicStartBotRound()
    {
        //GetNode<Label>("Bot/ValueLabel").Visible = true;
    }

    private void OnStopsButtonPressed()
    {
        ToggleDecisionMenuButtonsVisibility(false);
        _gameLogic.BotRound();
    }

    private void OnNewGameButtonPressed()
    {
        GetNode<ColorRect>("%GameOverMenu").Visible = false;
        _gameLogic.StartNewGame();
    }

    private void OnBackToMenuButtonPressed()
    {
        GetTree().ChangeSceneToPacked(MainMenuScene);
    }
}