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
    private int _wager = 10;
    public int PlayerCardsValue => _humanPlayer.CardsValueInHand;

    public int Wager
    {
        get
        {
            return _wager;
        }
        set
        {
            if (value < 0)
            {
                _wager = 0;
            }
            else if (_humanPlayer.Money < value)
            {
                _wager = _humanPlayer.Money;
            }
            else _wager = value;
            OnWagerChanged?.Invoke(_wager);
            
        }
    }

    public event Action<int>  OnSelectWager;
    public event Action<int> OnWagerChanged;
    public event Action<int> OnAskPlayerForMove;

    public event Action OnRoundOver;
    public event Action OnContinueGameAfterDeal;
    public event Action OnStartBotRound;
    public _21GameLogic( Player humanPlayer, Bot bot, int originalMoney )
    { 
        _humanPlayer = humanPlayer;
        _bot = bot;
        _originalMoney = originalMoney;
        _allPlayers = new List<EntityBase>() { _humanPlayer, _bot };
        _dealer = new Dealer(_allPlayers);
    }
    
    private void ResetGame()
    {
        _allPlayers.ForEach(player => player.ResetGameState());
        ResetRound();
    }
    private void ResetRound()
    {
        _dealer.Reset();
        _allPlayers.ForEach(player => player.ResetRoundState());
    }

    public async Task StartNewRound()
    {
        ResetRound();
        //OnReset?.Invoke();
        await _dealer.DealCard(_bot,false);
        await _dealer.DealCard(_humanPlayer,true);
        OnSelectWager?.Invoke(_wager);
        
    }

    public async Task SecondDeal()
    {
        await _dealer.DealCard(_humanPlayer,true);
        OnAskPlayerForMove?.Invoke(_humanPlayer.CardsValueInHand);
    }

    private void OnRoundWin(EntityBase winner)
    {
        if (winner is null) return;
        
        int winnerId = winner.Id;
        _allPlayers[winnerId].Money += Wager;
        for (int i = 0; i < 2; i++)
        {
            if(i==winnerId) continue;
            _allPlayers[i].Money -= Wager;
        }
    }

    public async Task NDeal()
    {  
        await _dealer.DealCard(_humanPlayer, true);
        if (_humanPlayer.CardsValueInHand > 21)
        {
            OnRoundWin(_bot);
            OnRoundOver?.Invoke();
        }
        else OnContinueGameAfterDeal?.Invoke();
    }

    public async Task BotRound()
    {
        OnStartBotRound?.Invoke();
        await _dealer.DealCard(_bot, false);
        while (_bot.DecideToGetNewCard())
        {
            await _dealer.DealCard(_bot, false);
        }

        _bot.ShowCards();
        OnRoundWin(DecideWhoWonRound());
        OnRoundOver?.Invoke();
    }

    private EntityBase DecideWhoWonRound()
    {
        int botValue = _bot.CardsValueInHand;
        int playerValue = _humanPlayer.CardsValueInHand;
        if (botValue > 21) return _humanPlayer;
        if (botValue > playerValue) return _bot;
        if (botValue < playerValue) return _humanPlayer;
        if (botValue == playerValue) return null;
        else throw new Exception("nem kene ide jutni");
    }

   
}