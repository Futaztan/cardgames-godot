using System.Collections.Generic;
using System.Linq;
using cardgames.Games.Lorum.Scripts.Cards;
using cardgames.Games.Lorum.Scripts.Players;
using cardgames.Lorum.Scripts.Cards;
using cardgames.Lorum.Scripts.UI;
using cardgames.Lorum.Scripts.UI.Elements;
using cardgames.Settings.Scripts;
using Godot;

namespace cardgames.Games.Lorum.Scripts;

public partial class Lorum : Control
{
    public static List<Cell> CenterCells = new List<Cell>();
    [Export] public StartingCardLabel StartingValueLabel { get; set; }
    [Export] public Pass PassIcon { get; set; }
    private LorumGameLogic _gameLogic;
    [Export] private PackedScene PointLabelScene { get; set; }
    [Export] private PackedScene MainMenuScene {get; set;}
    private List<RichTextLabel> _pointLabels = new List<RichTextLabel>();
    
    //TODO uj kor meg exit gomb rajta van a player labelen
    //TODO: refactor az animacio mitn zsirba, entitybasebe egy playcard fv
    /* 1. zold
     *  2. piros
     * 3. makk
     * 4. tok
     *
     */


    //ha roundsuntilend = 0, majd -1 akkor amíg el nem fogynak a pontok
  

    private void OnLogicRoundStarted(int startingValue)
    {
        StartingValueLabel.setText(startingValue);
    }

    private void OnLogicPlayerTurnPassed()
    {
        GD.Print("Player passz");
    }

    private void OnLogicPlayerTurnStarted()
    {
        GD.Print("player jon");
      /*  foreach (PlayerCard item in _gameLogic.HumanPlayer.CardsInHands)
        {
            item.CardClicked -= OnPlayerCardClicked;
            item.CardClicked += OnPlayerCardClicked;
        }*/
        _gameLogic.HumanPlayer.EnableCards();
    }


    public async override void _Ready()
    {
        SettingsValues settingsValues = SettingsManager.Instance.LoadSettings();
        int roundsLength = settingsValues.LorumLength;
        if (roundsLength == 0) roundsLength = -1;
        _gameLogic = new LorumGameLogic { StartingScore = settingsValues.LorumPoints, RoundsUntilEnd = roundsLength };
        CardDatabase.loadTextures();
        SetupNodesFromScene();
        SetupCellNodes();
        CreatePlayers(settingsValues.GlobalName);
        _gameLogic.OnRoundStarted += OnLogicRoundStarted;
        _gameLogic.OnGameOver += OnLogicGameOver;
        _gameLogic.OnPlayerTurnStarted += OnLogicPlayerTurnStarted;
        _gameLogic.OnPlayerTurnPassed += OnLogicPlayerTurnPassed;
        _gameLogic.OnReset += OnLogicReset;
        _gameLogic.OnRoundOver += OnLogicRoundOver;
        _gameLogic.AfterCardsDealed += AfterLogicCardsDealed;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _gameLogic.StartNewRound();
    }

    private void AfterLogicCardsDealed()
    {
        _gameLogic.HumanPlayer.DisableCards();

        foreach (PlayerCard item in _gameLogic.HumanPlayer.GetCardNodes())
        {
            GD.Print("siker");
            item.CardClicked += OnPlayerCardClicked;
        }

    }
    private void SetupNodesFromScene()
    {
        int score = _gameLogic.StartingScore;
        //StartingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");

        //PassIcon = GetNode<Pass>("PassIcon");
        PassIcon.PivotOffset = PassIcon.Size * 0.5f;
        
        var pointLabel = GetNode<RichTextLabel>("%PlayerPointLabel");
        _pointLabels.Add(pointLabel);
       
        for (int i = 1; i <= 3; i++)
        {
            string str = "%Bot" + i + "PointLabel";
            pointLabel = GetNode<RichTextLabel>(str);
            _pointLabels.Add(pointLabel);
        }

        Container box = GetNode<Container>("%PlayerScrollContainer");
        _pointLabels[0].Text = "[b]" + "PLAYER" + "\n" + score + " pont [/b]";
        _pointLabels[0].Size = _pointLabels[0].GetMinimumSize();
        _pointLabels[0].SetPosition(new Vector2(0,
            box.GlobalPosition.Y - _pointLabels[0].Size.Y - _pointLabels[0].Size.Y * 0.5F));

        box = GetNode<Container>("%Bot1Container");
        _pointLabels[1].Text = "[b]" + "BOT1" + "\n" + score + " pont [/b]";
        _pointLabels[1].Size = _pointLabels[1].GetMinimumSize();
        _pointLabels[1].SetPosition(new Vector2(0, box.GlobalPosition.Y - _pointLabels[1].Size.Y));

        box = GetNode<Container>("%Bot2Container");
        _pointLabels[2].Text = "[b]" + "BOT2" + "\n" + score + " pont [/b]";
        _pointLabels[2].Size = _pointLabels[2].GetMinimumSize();
        _pointLabels[2].SetPosition(new Vector2(box.Size.X * 0.5f - _pointLabels[2].Size.X * 0.5f,
            box.Size.Y - Mathf.Abs(box.GlobalPosition.Y)));

        box = GetNode<Container>("%Bot3Container");
        _pointLabels[3].Text = "[b]" + "BOT3" + "\n" + score + " pont [/b]";
        _pointLabels[3].Size = _pointLabels[3].GetMinimumSize();
        Vector2 size = GetViewport().GetVisibleRect().Size;

        _pointLabels[3]
            .SetPosition(new Vector2(size.X - _pointLabels[3].Size.X, box.GlobalPosition.Y - _pointLabels[3].Size.Y));
    }


    private void SetupCellNodes()
    {
        ////// CELLÁK
        HBoxContainer hbox = GetNode<HBoxContainer>("%GameAreaTopHBox");
        foreach (Cell child in hbox.GetChildren())
        {
            CenterCells.Add(child);
        }

        hbox = GetNode<HBoxContainer>("%GameAreaBottomHBox");
        foreach (Cell child in hbox.GetChildren())
        {
            CenterCells.Add(child);
        }
    }

    private void CreatePlayers(string playerName)
    {
        CardContainer container0 = GetNode<CardContainer>("%PlayerHBox");
        CardContainer container1 = GetNode<CardContainer>("%Bot1Container");
        CardContainer container2 = GetNode<CardContainer>("%Bot2Container");
        CardContainer container3 = GetNode<CardContainer>("%Bot3Container");


        var player = new Player(playerName, _gameLogic.StartingScore, _pointLabels[0], container0,PassIcon);
        var bots = new List<Bot>
        {
            new Bot("bot1", _gameLogic.StartingScore, _pointLabels[1], container1, PassIcon),
            new Bot("bot2", _gameLogic.StartingScore, _pointLabels[2], container2,PassIcon),
            new Bot("bot3", _gameLogic.StartingScore, _pointLabels[3], container3,PassIcon)
        };

        _gameLogic.SetupPlayers(player, bots);
    }

    private void OnPlayerCardClicked(PlayerCard card)
    {
       // _gameLogic.HumanPlayer.disableCards();
        _gameLogic.PlayHumanCard(card);
    }

    public void OnNewRoundButtonPressed()
    {
        ToggleUiVisibility(false);
        foreach (EntityBase item in _gameLogic.AllPlayers)
        {
            item.UpdateLabel();
        }
        _gameLogic.StartNewRound();
    }

    private void OnLogicReset()
    {
        StartingValueLabel.removeText();
        foreach (Cell item in CenterCells)
        {
            item.resetCell();
        }
    }
    
    private void OnLogicRoundOver()
    {
        HBoxContainer buttonContainer = GetNode<HBoxContainer>("%ButtonsContainer");
        buttonContainer.Visible = true;
    }


    private async void OnLogicGameOver(List<EntityBase> entities)
    {
        GD.Print("GAME OVER");

        for (int i = 0; i < entities.Count; i++)
        {
            AddRowToGrid(i+1, entities[i].Name, entities[i].Score);
        }

        ToggleUiVisibility(true);

    }

    private void ToggleUiVisibility(bool isGameOver)
    {
        bool toHide = !isGameOver;
        VBoxContainer center = GetNode<VBoxContainer>("%Center");
        Control gameResults = GetNode<Control>("%GameResults");
        HBoxContainer buttonContainer = GetNode<HBoxContainer>("%ButtonsContainer");
        _pointLabels.ForEach(p => p.Visible = toHide );
        this.GetChildren().OfType<Container>().ToList().ForEach(c => c.Visible = toHide);
        center.Visible = toHide;
        gameResults.Visible = !toHide;
        buttonContainer.Visible = !toHide;
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
    
    
    private void OnExitButtonPressed()
    {
        GetTree().ChangeSceneToPacked(MainMenuScene);
        //QueueFree();
    }
}