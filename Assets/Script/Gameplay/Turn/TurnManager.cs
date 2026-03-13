using System;
using UnityEngine;

public class TurnManager : MonoBehaviour, IServiceMB, ITurnManagerService
{
    public int CurrentPlayerID { get; private set; } = 1;
    public int TurnNumber { get; private set; } = 1;

    public event Action OnTurnChanged;

    private void Awake()
    {
        Register();
    }

    private void Start()
    {
        Utils.ColorLog($"Game Begin ! Player {CurrentPlayerID}, it's your turn ({TurnNumber}) ");
    }

    public void Register()
    {
        ServiceLocator.Register<ITurnManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<ITurnManagerService>(this);
    }

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
