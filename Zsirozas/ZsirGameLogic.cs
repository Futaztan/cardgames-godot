using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cardgames.Zsirozas;
using cardgames.Zsirozas.Players;
using Godot;

namespace zsir;

public class ZsirGameLogic
{
    public List<EntityBase> AllPlayers { get; private set; } = new();
    public List<Bot> Bots { get; private set; } = new();
    public Player HumanPlayer { get; private set; }
    public List<CardBase> CardsInArea { get; } = new();
    private Dealer _dealer;

    private int _roundLength = 0;

    public int WhoStarted { get; set; } = -1;
    public int StartingCardValueMod => StartingCardValue % 10;
    public static int StartingCardValue { get; set; } = -1;

    public EntityBase StartingPlayer => AllPlayers[WhoStarted];
    

    public event Action<EntityBase, int> OnRoundStarted; // Ki kezd, mi a kezdő érték
    public event Action<EntityBase, CardBase> OnCardPlayed; // Ki rakott kártyát
    public event Action OnReset; // Ki rakott kártyát
    public event Func<EntityBase, Task> OnRoundEnded; // Ki vitte el a kört
    public event Action<EntityBase, int> OnGameOver; // Győztes, Játékos helyezése
    public event Action OnPlayerTurnStarted; // Amikor a humán játékos következik

    public ZsirGameLogic(Player player, List<Bot> bots)
    {
        HumanPlayer = player;
        Bots = bots;
        AllPlayers = new List<EntityBase> { player };
        AllPlayers.AddRange(bots);
        _dealer = new Dealer(AllPlayers);
    }

    public EntityBase RoundWinner
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
    public async void StartNewGame()
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
            PlayBotCard(WhoStarted - 1, true);
            NextPlayerLoop(WhoStarted);
        }
    }

    public void PlayHumanCard(PlayerCard card)
    {
        CardsInArea.Add(card);
        _roundLength++;
        if (StartingCardValue == -1)
        {
            StartingCardValue =  HumanPlayer.PlayRound(card);
        
            OnRoundStarted?.Invoke(HumanPlayer, StartingCardValue);
        }
        else
        {
            HumanPlayer.PlayRound(card);
        }

        NextPlayerLoop(0);
    }


    private async Task NextPlayerLoop(int fromWho)
    {
        int botIdx = fromWho;

        while (botIdx < 3)
        {
            await Task.Delay(1400);
            if (StartingPlayer.Equals(Bots[botIdx]))
            {
                if (RoundWinner.Equals(StartingPlayer))
                    await RoundEnd();
                else
                {
                    var card = PlayBotCard(botIdx, false);
                    if (card is null) await RoundEnd();
                    else await NextPlayerLoop(Bots[botIdx].Id);
                }

                return;
            }
            PlayBotCard(botIdx, true);
            botIdx++;
        }

        await Task.Delay(1400);
        if (StartingPlayer.Equals(HumanPlayer) &&
            (!HumanPlayer.DoHavePlayableCard() || RoundWinner.Equals(HumanPlayer)))
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
            var (winner, position) = GetGameResult();
            OnGameOver?.Invoke(winner, position);
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
        if (starterEntity is cardgames.Zsirozas.Players.Player)
        {
            OnPlayerTurnStarted?.Invoke();
        }
        else if (starterEntity is Bot bot)
        {
            PlayBotCard(starterEntity.Id - 1, true);
            NextPlayerLoop(WhoStarted);
        }
    }

    public CardBase PlayBotCard(int botIdx, bool mustPlay)
    {
        if (StartingCardValue == -1)
        {
            int startingValue = StartingCardValue;

            CardBase selectedCard = Bots[botIdx].StartRound(ref startingValue);
            CardsInArea.Add(selectedCard);

            StartingCardValue = startingValue;
            OnRoundStarted?.Invoke(Bots[botIdx], StartingCardValue);
            return selectedCard;
        }
        else
        {
            var botCard = Bots[botIdx].NormalRound(StartingCardValue, mustPlay);
            if (botCard != null)
            {
                GD.Print(Bots[botIdx].Name + " jön");
                CardsInArea.Add(botCard);
                OnCardPlayed?.Invoke(Bots[botIdx], botCard);
            }

            return botCard;
        }
    }

    public void ResetRoundState()
    {
        StartingCardValue = -1;
        _roundLength = 0;
        CardsInArea.Clear();
    }

    public void ResetGameState()
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

    public bool IsGameOver()
    {
        return AllPlayers.Sum(p => p.CardsInHands.Count) == 0;
    }

    public (EntityBase Winner, int PlayerPosition) GetGameResult()
    {
        int playerScore = HumanPlayer.Score;
        int position = 1;
        EntityBase winner = AllPlayers[0];
        int maxScore = winner.Score;

        for (int i = 1; i < AllPlayers.Count; i++)
        {
            int currentScore = AllPlayers[i].Score;
            if (maxScore < currentScore)
            {
                maxScore = currentScore;
                winner = AllPlayers[i];
            }

            if (playerScore < currentScore) position++;
        }

        return (winner, position);
    }
}