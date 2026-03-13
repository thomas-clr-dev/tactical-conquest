using System;
using UnityEngine;

public interface ITurnManagerService
{
    int CurrentPlayerID { get; }
    int TurnNumber { get; }

    public event Action OnTurnChanged;
    void EndTurn();
}
