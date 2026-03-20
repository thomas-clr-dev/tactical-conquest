using System;
using UnityEngine;

public interface IEconomyManagerService
{
    public event Action OnEconomyInitialized;

    public int GetPlayerGold(int currentPlayer);

}
