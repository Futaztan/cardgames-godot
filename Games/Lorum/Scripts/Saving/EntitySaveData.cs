using System.Collections.Generic;
using cardgames.Games.Lorum.Scripts.Cards;

namespace cardgames.Games.Lorum.Scripts.Saving;

public class EntitySaveData
{
    public int Score { get; set; }
    public List<CardSaveData> CardsInHand { get; set; }
    
}