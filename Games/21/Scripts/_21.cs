using Godot;
using System;
using System.Collections.Generic;
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
        CreatePlayers();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _gameLogic.StartNewGame();
    }
    private void CreatePlayers()
    {
        CardContainer container0 = GetNode<CardContainer>("%Player");
        CardContainer container1 = GetNode<CardContainer>("%Dealer");

        int money = 100;

        var player = new Player("player", 0, container0, money);
        var bot = new Bot("bot", 1, container1, money);
        _gameLogic = new _21GameLogic(player,bot, money);
    }
}
