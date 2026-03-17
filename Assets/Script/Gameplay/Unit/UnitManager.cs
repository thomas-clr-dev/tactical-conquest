using System;
using UnityEngine;

public class UnitManager : MonoBehaviour, IServiceMB, IUnitManagerService
{
    private ITileManagerService _tileManagerService;

    public void Register()
    {
        ServiceLocator.Register<IUnitManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IUnitManagerService>(this);
    }

    private void Start()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _tileManagerService = ServiceLocator.Get<ITileManagerService>();

        if (_tileManagerService != null)
        {
            _tileManagerService.OnBaseGenerated += OnTroopsGeneration;
            Utils.ColorLog("UnitManager: Subscribed to OnBaseGenerated", "Green");
        }
    }

    private void OnTroopsGeneration(int playerId)
    {
        if (playerId == 1)
        {
            Utils.ColorLog("Player 1 unit génération ...", "Cyan");
        }
        else if (playerId == 2)
        {
            Utils.ColorLog("Player 2 unit génération ...", "Purple");
        }
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;

        if (_tileManagerService != null)
        {
            _tileManagerService.OnBaseGenerated -= OnTroopsGeneration;
        }

        Unregister();
    }

}
