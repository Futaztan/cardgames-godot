using System.Collections.Generic;

namespace cardgames.Games.Lorum.Scripts.Saving;

public class GameSaveData
{
    public int WhoStarted { get; set; }
    public List<EntitySaveData> AllPlayers { get; set; }
}