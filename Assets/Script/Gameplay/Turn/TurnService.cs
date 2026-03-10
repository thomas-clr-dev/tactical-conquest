using System;
using UnityEngine;

public class TurnService : ITurnService
{
    public int CurrentPlayerID { get; private set; } = 1;
    public int TurnNumber { get; private set; } = 1;

    public event Action OnTurnChanged;

    public void EndTurn()
    {
        CurrentPlayerID = (CurrentPlayerID == 1) ? 2 : 1;

        if (CurrentPlayerID == 1)
        {
            TurnNumber++;
        }

        Utils.ColorLog($"Turn change ! Player {CurrentPlayerID}, it's your turn ({TurnNumber}) ");

        OnTurnChanged?.Invoke();
    }
}
