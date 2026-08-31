using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cardgames.Games.Zsirozas.Scripts.Cards;
using cardgames.Games.Zsirozas.Scripts.Players;
using Godot;
using zsir;

namespace cardgames.Games.Zsirozas.Scripts;

public class ZsirGameLogic
{
    private List<EntityBase> AllPlayers { get; }
    private List<Bot> Bots { get;  }
    public Player HumanPlayer { get;  }
    public List<CardBase> CardsInArea { get; } = new();
    private Dealer _dealer;

    private int _roundLength = 0;
    private const int WaitMillisAfterCardPlace = 400;

    private int WhoStarted { get; set; } = -1;
    public int StartingCardValueMod => StartingCardValue % 10;
    public static int StartingCardValue { get; set; } = -1;

    public EntityBase StartingPlayer => AllPlayers[WhoStarted];
    

    public event Action<EntityBase, int> OnRoundStarted; // Ki kezd, mi a kezdő érték
    public event Action<EntityBase, CardBase> OnCardPlayed; // Ki rakott kártyát
    public event Action OnReset; // Ki rakott kártyát
    public event Func<EntityBase, Task> OnRoundEnded; // Ki vitte el a kört
    public event Action<List<EntityBase>> OnGameOver; // Győztes, Játékos helyezése
    public event Action OnPlayerTurnStarted; // Amikor a humán játékos következik

    public ZsirGameLogic(Player player, List<Bot> bots)
    {
        HumanPlayer = player;
        Bots = bots;
        AllPlayers = new List<EntityBase> { player };
        AllPlayers.AddRange(bots);
        _dealer = new Dealer(AllPlayers);
    }

    private EntityBase RoundWinner
    {
        get
        {
            if (!CardsInArea.Any()) return null;

            EntityBase roundWinner = StartingPlayer;
            int starterCardValue = CardsInArea.First().getValue();
            for (int i = 0; i < CardsInArea.Count; i++)
            {
                int currentCardValue = CardsInArea[i].getValue();
                if (IsSameType(currentCardValue, starterCardValue) || IsVII(currentCardValue))
                {
                    roundWinner = AllPlayers[(WhoStarted + i) % 4];
                    GD.Print("JELENLEGI NYERTES: " + roundWinner.Name + " " + i);
                }
            }

            return roundWinner;
        }
    }
    public async Task StartNewGame()
    {
        ResetGameState();
        OnReset?.Invoke();
        AllPlayers.ForEach(player => player.ResetState());
        await _dealer.DealCardsToPlayers(4);
        HumanPlayer.DisableCards();
        WhoStarted = new Random().Next(0, 4);
        // WhoStarted = 1;
        //Task.Delay(1300);
        GD.Print(StartingPlayer.Name + " kezd");
        if (WhoStarted == 0)
        {
            //OnRoundStarted?.Invoke(HumanPlayer, StartingCardValue);
            OnPlayerTurnStarted?.Invoke();
        }
        else
        {
            await PlayBotCard(WhoStarted - 1, true);
            NextPlayerLoop(WhoStarted);
        }
    }

    public async Task PlayHumanCard(PlayerCard card)
    {
        CardsInArea.Add(card);
        _roundLength++;
        if (StartingCardValue == -1)
        {
            StartingCardValue = card.getValue();
            OnRoundStarted?.Invoke(HumanPlayer, StartingCardValue);
            await HumanPlayer.PlayCard(card);
            
            //await Task.Delay(WaitMillisAfterCardPlace);
        }
        else
        {
            await HumanPlayer.PlayCard(card);
            //await Task.Delay(WaitMillisAfterCardPlace);
        }

        NextPlayerLoop(0);
    }


    private async Task NextPlayerLoop(int fromWho)
    {
        int botIdx = fromWho;

        while (botIdx < 3)
        {
            //await Task.Delay(1400);
            if (StartingPlayer.Equals(Bots[botIdx]))
            {
                if (RoundWinner.Equals(StartingPlayer))
                    await RoundEnd();
                else
                {
                    var card = await PlayBotCard(botIdx, false);
                    if (card == null) await RoundEnd();
                    else await NextPlayerLoop(Bots[botIdx].Id);
                }

                return;
            }
            await PlayBotCard(botIdx, true);
            botIdx++;
        }

        //await Task.Delay(1400);
        if (StartingPlayer.Equals(HumanPlayer) &&
            (!HumanPlayer.HavePlayableCard() || RoundWinner.Equals(HumanPlayer)))
        {
            await RoundEnd();
        }
        else OnPlayerTurnStarted?.Invoke();
    }

    public async Task RoundEnd()
    {
        RoundWinner.CollectedCards.AddRange(CardsInArea);
        GD.Print("kör nyertes: " + RoundWinner.Name);
        if (OnRoundEnded != null)
        {
             await OnRoundEnded(RoundWinner);
        }
        if (IsGameOver())
        {
            var orderedListByScore = AllPlayers.OrderByDescending(p => p.Score).ToList();
            OnGameOver?.Invoke(orderedListByScore);
        }
        else StartNewRound(RoundWinner);
    }

    private async void StartNewRound(EntityBase starterEntity)
    {
      
        OnReset?.Invoke();
        await _dealer.DealCardsToPlayers(_roundLength);
        ResetRoundState();
        HumanPlayer.DisableCards();
        //foreach (var p in AllPlayers) p.NewRoundDeal(this, maxCardNumForPlayer);
        WhoStarted = starterEntity.Id;
        if (starterEntity is Player)
        {
            OnPlayerTurnStarted?.Invoke();
        }
        else if (starterEntity is Bot bot)
        {
            await PlayBotCard(starterEntity.Id - 1, true);
            NextPlayerLoop(WhoStarted);
        }
    }

    private async Task<CardBase> PlayBotCard(int botIdx, bool mustPlay)
    {
        if (StartingCardValue == -1)
        {
            //CardBase selectedCard = await Bots[botIdx].StartRound();
            CardBase startingCard = Bots[botIdx].SelectPlayedCard(-1);
            StartingCardValue = startingCard.getValue();
            OnRoundStarted?.Invoke(Bots[botIdx], StartingCardValue);
            CardsInArea.Add(startingCard);
            await Bots[botIdx].PlayCard(startingCard);
            //await Task.Delay(WaitMillisAfterCardPlace);
            return startingCard;
        }
        else
        {
            //CardBase selectedCard = await Bots[botIdx].NormalRound(StartingCardValue, mustPlay);
            CardBase selectedCard = Bots[botIdx].SelectPlayedCard(StartingCardValue, mustPlay);
            if (selectedCard != null)
            {
                GD.Print(Bots[botIdx].Name + " jön");
                CardsInArea.Add(selectedCard);
                await Bots[botIdx].PlayCard(selectedCard);
                //OnCardPlayed?.Invoke(Bots[botIdx], botCard); does not have any use
            }
            //await Task.Delay(WaitMillisAfterCardPlace);
            return selectedCard;
        }
    }

    private void ResetRoundState()
    {
        StartingCardValue = -1;
        _roundLength = 0;
        CardsInArea.Clear();
    }

    private void ResetGameState()
    {
        _dealer.Reset();
        ResetRoundState();
    }

    private bool IsSameType(int value, int startingCardValue)
    {
        return (value % 10) == (startingCardValue % 10);
    }

    private bool IsVII(int value)
    {
        if (value % 10 == 5) return true;
        return false;
    }

    private bool IsGameOver()
    {
        return AllPlayers.Sum(p => p.CardsInHands.Count) == 0;
    }
    
}