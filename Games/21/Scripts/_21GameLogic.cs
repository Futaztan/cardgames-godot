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
    
    private int MaxWager => Math.Min(_humanPlayer.Money, _bot.SpecifyMaxWager());

    public int Wager
    {
        get => _wager;
        set
        {
            if (value < 1)
            {
                _wager = 1;
            }
            else if (MaxWager < value)
            {
                _wager = MaxWager;
            }
            else _wager = value;
            OnWagerChanged?.Invoke(_wager);
            
        }
    }

    public event Action<int, int>  OnSelectWager;
    public event Action<int> OnWagerChanged;
    public event Action<int> OnAskPlayerForMove;
    public event Action OnReset;
    public event Action OnRoundOver;
    public event Action OnContinueGameAfterDeal;
    public event Action OnStartBotRound;
    public event Action<EntityBase> OnGameOver;
    public _21GameLogic( Player humanPlayer, Bot bot, int originalMoney )
    { 
        _humanPlayer = humanPlayer;
        _bot = bot;
        _originalMoney = originalMoney;
        _allPlayers = new List<EntityBase>() { _humanPlayer, _bot };
        _dealer = new Dealer(_allPlayers);
    }
    
    private void ResetRound()
    {
        _dealer.Reset();
        _allPlayers.ForEach(player => player.ResetRoundState());
    }
    public void StartNewGame()
    {
        _allPlayers.ForEach(p=> p.Money = _originalMoney);
        StartNewRound();
    }

    public async Task StartNewRound()
    {
        ResetRound();
        OnReset?.Invoke();
        await _dealer.DealCard(_bot,false);
        await _dealer.DealCard(_humanPlayer,true);
  
        OnSelectWager?.Invoke(Wager, MaxWager);
        
    }

    public async Task SecondDeal()
    {
        await _dealer.DealCard(_humanPlayer,true);
        if (_humanPlayer.StartedWithTwoAces())
        {
            HandleRoundOver(_humanPlayer);
        }
        else OnAskPlayerForMove?.Invoke(_humanPlayer.CardsValueInHand);
    }

    private void OnRoundWin(EntityBase winner)
    {
        _bot.ShowCards();
        if (winner is null) return;
        int winnerId = winner.Id;
        _allPlayers[winnerId].Money += Wager;
        _allPlayers[winnerId].SetLabelColor("#99ff99");
        for (int i = 0; i < _allPlayers.Count; i++)
        {
            if(i==winnerId) continue;
            _allPlayers[i].Money -= Wager;
            _allPlayers[i].SetLabelColor("#ff9999");
            _allPlayers[i].AnimateCoins(_allPlayers[winnerId].GoldCoinTexture);
        }
    }

    private bool IsGameOver()
    {
        foreach (var entity in _allPlayers)
        {
            if (entity.Money <= 0) return true;
        }

        return false;
    }

    public async Task NDeal()
    {  
        await _dealer.DealCard(_humanPlayer, true);
        if (_humanPlayer.CardsValueInHand > 21)
        {
            HandleRoundOver(_bot);
        }
        else OnContinueGameAfterDeal?.Invoke();
    }

    private void HandleRoundOver(EntityBase winner)
    {
        OnRoundWin(winner);
        if (IsGameOver())
        {
            OnGameOver?.Invoke(winner);
        }
        else OnRoundOver?.Invoke();
    }
    public async Task BotRound()
    {
        OnStartBotRound?.Invoke();
        await _dealer.DealCard(_bot, false);
        while (_bot.DecideToGetNewCard())
        {
            await _dealer.DealCard(_bot, false);
        }

        var winner = DecisionWhoWonRound();
        HandleRoundOver(winner);
    }

    private EntityBase DecisionWhoWonRound()
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