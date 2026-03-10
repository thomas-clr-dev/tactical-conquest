using System;
using UnityEngine;

public interface ITurnService
{
    int CurrentPlayerID { get; }
    int TurnNumber { get; }
    public event Action OnTurnChanged;
    void EndTurn();
}
