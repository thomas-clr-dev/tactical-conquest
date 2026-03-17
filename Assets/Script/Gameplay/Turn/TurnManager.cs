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
        // Logique de démarrage ici si nécessaire
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

        OnTurnChanged?.Invoke();
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }
}
