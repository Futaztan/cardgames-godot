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

    public event Action OnPlayerBusted;
    public event Action OnContinueGameAfterDeal;
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
        OnSelectWager?.Invoke(_wager);
        
    }

    public async Task SecondDeal()
    {
        await _dealer.DealCard(_humanPlayer,true);
        OnAskPlayerForMove?.Invoke(_humanPlayer.CardsValueInHand);
    }

    private void OnRoundWin(EntityBase winner)
    {
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
            OnPlayerBusted?.Invoke();
        }
        else OnContinueGameAfterDeal?.Invoke();
    }

    public void ResetRound()
    {
        
    }
}