using Godot;
using System;
using System.Collections.Generic;
using cardgames.Lorum.Scripts;
using cardgames.Lorum.Scripts.Cards;
using cardgames.Lorum.Scripts.Players;

namespace lorum;

public partial class Lorum : Control
{
    public static List<Cell> CenterCells = new List<Cell>();
    private StartingCardLabel _startingValueLabel;
    public static Pass PassIcon;
    private Button testbutton;
    private LorumGameLogic _gameLogic;
    private PackedScene _pointLabelScene;
    private List<RichTextLabel> _pointLabels = new List<RichTextLabel>();

    //TODO diflabelek utan elbaszodik a label meret és nagyobb lesz,  ??? nem talaltam meg megint ezt


    /* 1. zold
     *  2. piros
     * 3. makk
     * 4. tok
     *
     */


    //ha roundsuntilend = 0, majd -1 akkor amíg el nem fogynak a pontok
    public void init(int score, int roundsUntilEnd)
    {
        if (roundsUntilEnd == 0) roundsUntilEnd = -1;
        _gameLogic = new LorumGameLogic { StartingScore = score, RoundsUntilEnd = roundsUntilEnd };
    }

    private void OnLogicRoundStarted(int startingValue)
    {
        _startingValueLabel.setText(startingValue);
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
        CardDatabase.loadTextures();
        SetupNodesFromScene();
        SetupCellNodes();
        CreatePlayers();
        _gameLogic.OnRoundStarted += OnLogicRoundStarted;
        _gameLogic.OnGameOver += OnLogicGameOver;
        _gameLogic.OnPlayerTurnStarted += OnLogicPlayerTurnStarted;
        _gameLogic.OnPlayerTurnPassed += OnLogicPlayerTurnPassed;
        _gameLogic.OnReset += OnLogicReset;
        _gameLogic.OnRoundOver += OnLogicRoundOver;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _gameLogic.StartNewGame();
    }


    private void SetupNodesFromScene()
    {
        int score = _gameLogic.StartingScore;
        _startingValueLabel = GetNode<StartingCardLabel>("Center/HBoxContainer/StartingCardLabel");
        testbutton = GetNode<Button>("Button");

        PassIcon = GetNode<Pass>("PassIcon");
        PassIcon.PivotOffset = PassIcon.Size * 0.5f;
        _pointLabelScene = (PackedScene)GD.Load("res://Lorum/Scenes/PointLabel.tscn");

        for (int i = 0; i < 4; i++)
        {
            RichTextLabel pointLabel = (RichTextLabel)_pointLabelScene.Instantiate();
            _pointLabels.Add(pointLabel);
            this.AddChild(pointLabel);
        }

        Container box = GetNode<Container>("PLAYER");
        _pointLabels[0].Text = "[b]" + "PLAYER" + "\n" + score + " pont [/b]";
        _pointLabels[0].Size = _pointLabels[0].GetMinimumSize();
        _pointLabels[0].SetPosition(new Vector2(0,
            box.GlobalPosition.Y - _pointLabels[0].Size.Y - _pointLabels[0].Size.Y * 0.5F));

        box = GetNode<Container>("BOT1");
        _pointLabels[1].Text = "[b]" + "BOT1" + "\n" + score + " pont [/b]";
        _pointLabels[1].Size = _pointLabels[1].GetMinimumSize();
        _pointLabels[1].SetPosition(new Vector2(0, box.GlobalPosition.Y - _pointLabels[1].Size.Y));

        box = GetNode<Container>("BOT2");
        _pointLabels[2].Text = "[b]" + "BOT2" + "\n" + score + " pont [/b]";
        _pointLabels[2].Size = _pointLabels[2].GetMinimumSize();
        _pointLabels[2].SetPosition(new Vector2(box.Size.X * 0.5f - _pointLabels[2].Size.X * 0.5f,
            box.Size.Y - Mathf.Abs(box.GlobalPosition.Y)));

        box = GetNode<Container>("BOT3");
        _pointLabels[3].Text = "[b]" + "BOT3" + "\n" + score + " pont [/b]";
        _pointLabels[3].Size = _pointLabels[3].GetMinimumSize();
        Vector2 size = GetViewport().GetVisibleRect().Size;

        _pointLabels[3]
            .SetPosition(new Vector2(size.X - _pointLabels[3].Size.X, box.GlobalPosition.Y - _pointLabels[3].Size.Y));
    }


    private void SetupCellNodes()
    {
        ////// CELLÁK
        HBoxContainer hbox = GetNode<HBoxContainer>("Center/HBoxContainer1");
        foreach (Cell child in hbox.GetChildren())
        {
            CenterCells.Add(child);
        }

        hbox = GetNode<HBoxContainer>("Center/HBoxContainer2");
        foreach (Cell child in hbox.GetChildren())
        {
            CenterCells.Add(child);
        }
    }

    private void CreatePlayers()
    {
        CardContainer container0 = GetNode<CardContainer>("PLAYER/HBoxContainer");
        CardContainer container1 = GetNode<CardContainer>("BOT1");
        CardContainer container2 = GetNode<CardContainer>("BOT2");
        CardContainer container3 = GetNode<CardContainer>("BOT3");


        var player = new Player("player", _gameLogic.StartingScore, _pointLabels[0], container0);
        var bots = new List<Bot>
        {
            new Bot("bot1", _gameLogic.StartingScore, _pointLabels[1], container1),
            new Bot("bot2", _gameLogic.StartingScore, _pointLabels[2], container2),
            new Bot("bot3", _gameLogic.StartingScore, _pointLabels[3], container3)
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
        ToggleNewRoundButton(false);
        ToggleExitButton(false);
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");
        center.Show();
        statCenter.Hide();
        foreach (EntityBase item in _gameLogic.AllPlayers)
        {
            item.UpdateLabel();
        }

        _gameLogic.StartNewGame();
    }

    private void OnLogicReset()
    {
        _startingValueLabel.removeText();

        foreach (Cell item in CenterCells)
        {
            item.resetCell();
        }


        _gameLogic.HumanPlayer.DisableCards();

        foreach (PlayerCard item in _gameLogic.HumanPlayer.GetCardNodes())
        {
            GD.Print("siker");
            item.CardClicked += OnPlayerCardClicked;
        }


        //await ToSignal(GetTree().CreateTimer(1f), "timeout");
    }

    private void OnLogicRoundOver()
    {
        ToggleNewRoundButton(true);
        ToggleExitButton(true);
    }


    private async void OnLogicGameOver((EntityBase Winner, int PlayerPosition) result)
    {
        GD.Print("GAME OVER");
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        RichTextLabel winLabel = GetNode<RichTextLabel>("StatCenter/WinLabel");
        VBoxContainer center = GetNode<VBoxContainer>("Center");
        VBoxContainer statCenter = GetNode<VBoxContainer>("StatCenter");
        center.Hide();
        statCenter.Show();
        winLabel.Text = result.Winner.Name + " NYERT!\n\n";
        winLabel.Text += "GRATULÁLUNK! \n" + result.PlayerPosition + ". LETTÉL!";
        ToggleExitButton(true);
        ToggleNewRoundButton(true);
        ToggleExitButton(true);
    }


    private void ToggleNewRoundButton(bool enabled)
    {
        Button newRoundButton = GetNode<Button>("ButtonsContainer/NewRoundButton");
        newRoundButton.Visible = enabled;
    }

    private void ToggleExitButton(bool enabled)
    {
        Button exitButton = GetNode<Button>("ButtonsContainer/ExitButton");
        exitButton.Visible = enabled;
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

    public void OnExitButtonPressed()
    {
        GD.Print("EXIT");
        ToggleExitButton(false);
        GetTree().ChangeSceneToFile("res://Lorum/Scenes/Menus/GameMenu.tscn");
        //QueueFree();
    }
}