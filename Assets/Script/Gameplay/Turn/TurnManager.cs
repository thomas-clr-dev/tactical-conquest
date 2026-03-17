using System;
using UnityEngine;

public class TurnManager : MonoBehaviour, IServiceMB, ITurnManagerService
{
    public int CurrentPlayerID { get; private set; } = 1;
    public int TurnNumber { get; private set; } = 1;

    public event Action OnTurnChanged;


    private void Start()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }
    private void OnGameReady()
    {
        Utils.ColorLog("GameManager: Game is ready!", "Cyan");
        // Logique de démarrage ici si nécessaire
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

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }
}
