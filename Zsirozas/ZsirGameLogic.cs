using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace zsir;

public class ZsirGameLogic
{
    public List<EntityBase> AllPlayers { get; private set; } = new();
    public List<Bot> Bots { get; private set; } = new();
    public Player HumanPlayer { get; private set; }
    public List<CardBase> CardsInArea { get; } = new();

    public int WhoStarted { get; set; } = -1;
    public int StartingCardValueMod => StartingCardValue % 10;
    public int StartingCardValue { get; set; } = -1;

    public EntityBase StartingPlayer => AllPlayers[WhoStarted];
    private List<int> _usedCardIndexes = new List<int>();
    private int DeckCardCount => 32 - _usedCardIndexes.Count;

    public event Action<int> OnRoundStarted; // Ki kezd, mi a kezdő érték
    public event Action<EntityBase, CardBase> OnCardPlayed; // Ki rakott kártyát
    public event Action OnReset; // Ki rakott kártyát
    public event Action<EntityBase> OnRoundEnded; // Ki vitte el a kört
    public event Action<EntityBase, int> OnGameOver; // Győztes, Játékos helyezése
    public event Action OnPlayerTurnStarted; // Amikor a humán játékos következik

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

    public int DrawCardIndex()
    {
        if (_usedCardIndexes.Count >= 32) return -1; // Elfogyott a pakli
        Random random = new();
        int rnd = random.Next(0, 32);
        while (_usedCardIndexes.Contains(rnd))
        {
            rnd = random.Next(0, 32);
        }

        _usedCardIndexes.Add(rnd);
        return rnd;
    }

    public void SetupPlayers(Player player, List<Bot> bots)
    {
        HumanPlayer = player;
        Bots = bots;
        AllPlayers = new List<EntityBase> { player };
        AllPlayers.AddRange(bots);
    }

    public void StartNewGame()
    {
        _usedCardIndexes.Clear();
        ResetRoundState();

        foreach (EntityBase player in AllPlayers)
        {
            player.ResetState();
            player.NewGameDeal(this);
        }

        OnReset?.Invoke();

        WhoStarted = new Random().Next(0, 4);
        // WhoStarted = 1;

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
        if (StartingCardValue == -1)
        {
            StartingCardValue =  HumanPlayer.StartRound(card);
        
            OnRoundStarted?.Invoke(StartingCardValue);
        }
        else
        {
            HumanPlayer.NormalRound(card);
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
                    RoundEnd();
                else
                {
                    var card = PlayBotCard(botIdx, false);
                    if (card is null) RoundEnd();
                    else NextPlayerLoop(Bots[botIdx].Id);
                }

                return;
            }


            PlayBotCard(botIdx, true);

            botIdx++;
        }

        await Task.Delay(1400);
        if (StartingPlayer.Equals(HumanPlayer) &&
            (!HumanPlayer.IsHavePlayableCard(StartingCardValue) || RoundWinner.Equals(HumanPlayer)))
        {
            RoundEnd();
        }
        else OnPlayerTurnStarted?.Invoke();
    }

    public void RoundEnd()
    {
        RoundWinner.CollectedCards.AddRange(CardsInArea);
        GD.Print("kör nyertes: " + RoundWinner.Name);
        if (IsGameOver())
        {
            var (winner, position) = GetGameResult();
            OnGameOver?.Invoke(winner, position);
        }
        else StartNewRound(RoundWinner);
    }

    private void StartNewRound(EntityBase starterEntity)
    {
        ResetRoundState();
        int maxCardNumForPlayer = DeckCardCount / 4;
        foreach (var p in AllPlayers) p.NewRoundDeal(this, maxCardNumForPlayer);

        OnReset?.Invoke();
        WhoStarted = starterEntity.Id;
        if (starterEntity is Player)
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
            OnRoundStarted?.Invoke(StartingCardValue);
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
        CardsInArea.Clear();
    }

    public void ResetGameState()
    {
        _usedCardIndexes.Clear();
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