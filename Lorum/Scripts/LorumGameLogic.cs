using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cardgames.Lorum.Scripts.Players;
using Godot;
using lorum;


namespace cardgames.Lorum.Scripts;

public class LorumGameLogic
{
    public List<EntityBase> AllPlayers { get; private set; } = new();
    public List<Bot> Bots { get; private set; } = new();
    public Player HumanPlayer { get; private set; }


    public int WhoStarted { get; set; } = -1;
    public static int StartingCardValueMod => StartingCardValue % 10;
    public static int StartingCardValue { get; set; } = -1;

    public EntityBase StartingPlayer => AllPlayers[WhoStarted];
    private readonly List<int> _usedCardIndexes = new List<int>();
    public int StartingScore { get; set; }
    public int RoundsUntilEnd { get; set; }

    public event Action<int> OnRoundStarted; // Ki kezd, mi a kezdő érték
    public event Action OnPlayerTurnStarted; // Amikor a humán játékos következik
    public event Action OnPlayerTurnPassed; // Amikor a humán játékos passzol
    public event Action<(EntityBase, int)> OnGameOver; // ha valakienk elfogy a pénze
    public event Action OnReset; // Ki rakott kártyát
    public event Action OnRoundOver;

    public void SetupPlayers(Player player, List<Bot> bots)
    {
        HumanPlayer = player;
        Bots = bots;
        AllPlayers = new List<EntityBase> { player };
        AllPlayers.AddRange(bots);
    }

    public int DrawCardIndex()
    {
        if (_usedCardIndexes.Count >= 32) throw new ArgumentException(); // Elfogyott a pakli
        Random random = new();
        int rnd = random.Next(0, 32);
        while (_usedCardIndexes.Contains(rnd))
        {
            rnd = random.Next(0, 32);
        }

        _usedCardIndexes.Add(rnd);
        return rnd;
    }

    public void StartNewGame()
    {
        _usedCardIndexes.Clear();
        ResetRoundState();

        foreach (EntityBase player in AllPlayers)
        {
            player.ResetState();
            player.DealNewCards(this);
        }

        OnReset?.Invoke();

        Task.Delay(1400);

        int whoStarts;
        if (WhoStarted == -1)
        {
            Random random = new Random();
            whoStarts = random.Next(0, 4);
        }
        else
        {
            whoStarts = WhoStarted + 1;
            if (whoStarts == 4) whoStarts = 0;
        }

        WhoStarted = whoStarts;


        if (WhoStarted == 0) OnPlayerTurnStarted?.Invoke();
        else
        {
            GD.Print(WhoStarted + ". bot kezd");
            StartingCardValue = Bots[WhoStarted - 1].StartRound();
            OnRoundStarted?.Invoke(StartingCardValue);
            NextPlayerLoop(WhoStarted);
        }
    }


    public void StartNewRound()
    {
        StartNewGame();
    }

    private void ResetRoundState()
    {
        StartingCardValue = -1;
    }
    //valid-e amire kattintott

    public void PlayHumanCard(PlayerCard card)
    {
        if (StartingCardValue == -1)
        {
            StartingCardValue = HumanPlayer.StartRound(card);

            OnRoundStarted?.Invoke(StartingCardValue);

            NextPlayerLoop(0);
            return;
        }

        int cardCount = HumanPlayer.NormalRound(card);
        if (cardCount >= 0)
        {
            if (cardCount == 0)
            {
                OnRoundWin(0);
            }
            else NextPlayerLoop(0);
        }
    }

    private async Task NextPlayerLoop(int fromWho)
    {
        int botIdx = fromWho;
        while (botIdx < 3)
        {
            await Task.Delay(1400);
            if (Bots[botIdx].PlayNormalRound() == 0)
            {
                OnRoundWin(botIdx + 1);
                return;
            }

            botIdx++;
        }

        await Task.Delay(1400);

        StartHumanPlayerRound();
    }

    private void StartHumanPlayerRound()
    {
        if (!HumanPlayer.CanPlaceCard())
        {
            OnPlayerTurnPassed?.Invoke();
            NextPlayerLoop(0);
        }
        else OnPlayerTurnStarted?.Invoke();
        // HumanPlayer.enableCards();
    }

    private void OnRoundWin(int winnerid)
    {
        int sumPoint = 0;
        for (int i = 0; i < 4; i++)
        {
            if (winnerid == i) continue;
            sumPoint += AllPlayers[i].CardsInHandCount;
            AllPlayers[i].OnLose();
        }

        AllPlayers[winnerid].OnWin(sumPoint);

        if (IsGameOver())
        {
            OnGameOver?.Invoke(GetGameResult());
        }
        else
        {
            OnRoundOver?.Invoke();
            //StartNewRound();
        }
    }

    private (EntityBase Winner, int PlayerPosition) GetGameResult()
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

    private bool IsGameOver()
    {
        if (RoundsUntilEnd == -1)
        {
            foreach (EntityBase item in AllPlayers)
            {
                if (item.Score <= 0)
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            RoundsUntilEnd--;
            return RoundsUntilEnd == 0;
        }
    }
}