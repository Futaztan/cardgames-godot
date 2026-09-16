using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cardgames.Games._21.Scripts.Players;
using Godot;

namespace cardgames.Games._21.Scripts;

public class _21GameLogic
{
    private List<EntityBase> _allPlayers;
    private Player _humanPlayer;
    private Bot _bot;
    private Dealer _dealer;
    private int _originalMoney;
    public _21GameLogic( Player humanPlayer, Bot bot, int originalMoney )
    { 
        _humanPlayer = humanPlayer;
        _bot = bot;
        _originalMoney = originalMoney;
        _allPlayers = new List<EntityBase>() { _humanPlayer, _bot };
        _dealer = new Dealer(_allPlayers);
    }
    
    private void ResetGameState()
    {
        _allPlayers.ForEach(player => player.ResetGameState());
        ResetRoundState();
    }
    private void ResetRoundState()
    {
        _dealer.Reset();
        _allPlayers.ForEach(player => player.ResetRoundState());
    }

    public async Task StartNewGame()
    {
        ResetGameState();
        //OnReset?.Invoke();
        await _dealer.DealCard(_bot,false);
        await _dealer.DealCard(_humanPlayer,true);
     
        
    }
}