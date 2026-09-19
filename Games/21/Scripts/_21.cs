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
        _gameLogic.OnContinueGameAfterDeal += OnLogicContinueGameAfterDeal;
        _gameLogic.OnStartBotRound += OnLogicStartBotRound;
        _gameLogic.StartNewRound();
    }

    private void ResetRound()
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

        int money = 100;

        var player = new Player("player", 0, container0, money, label0, valueLabel0);
        var bot = new Bot("bot", 1, container1, money, label1,valueLabel1);
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

    private void OnLogicSelectWager(int wager)
    {
        ColorRect menu = GetNode<ColorRect>("%WagerMenu");
        menu.Visible = true;
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
        ToggleDecisionMenuButtonsVisibility(false);
        GetNode<Container>("%NewRoundContainer").Visible = true;
    }

    private void OnNewRoundButtonPressed()
    {
        ResetRound();
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
        GetNode<Label>("Bot/ValueLabel").Visible = true;
    }

    private void OnStopsButtonPressed()
    {
        ToggleDecisionMenuButtonsVisibility(false);
        _gameLogic.BotRound();
    }
}