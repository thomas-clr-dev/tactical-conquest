using System;
using System.Dynamic;
using UnityEngine;

public class EconomyManager : MonoBehaviour, IServiceMB, IEconomyManagerService
{
    private ITurnManagerService _iTurnManagerService;
    private ITileManagerService _iTileManagerService;

    public int Player1Wallet = 5;
    public int Player2Wallet = 5;

    public event Action OnEconomyInitialized;

    public void Register()
    {
        ServiceLocator.Register<IEconomyManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IEconomyManagerService>(this);
    }

    private void Start()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();
        _iTileManagerService = ServiceLocator.Get<ITileManagerService>();

        _iTurnManagerService.OnTurnChanged += AddGold;

        Player1Wallet = 5;
        Player2Wallet = 5;

        OnEconomyInitialized?.Invoke();
    }

    private void AddGold()
    {
        int currentPlayer = _iTurnManagerService.CurrentPlayerID;
        int previousPLayer = currentPlayer == 1 ? 2 : 1;

        int conqueredTiles = _iTileManagerService.GetConqueredTileCount(previousPLayer);

        if (previousPLayer == 1)
        {
            int oldWallet = Player1Wallet;
            Player1Wallet += conqueredTiles;
        }
        else if (previousPLayer == 2)
        {
            int oldWallet = Player2Wallet;
            Player2Wallet += conqueredTiles;
        }
    }

    public int GetPlayerGold(int currentPlayer)
    {
        int activePlayerWallet = currentPlayer == 1 ? Player1Wallet : Player2Wallet;
        return activePlayerWallet;
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;

        if (_iTurnManagerService != null)
        {
            _iTurnManagerService.OnTurnChanged -= AddGold;
        }

        Unregister();
    }
}
