using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IServiceMB, IGameManagerReadService
{
    //private bool _isGameReady = false;

    private void Awake()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        //_isGameReady = true;
        Utils.ColorLog("GameManager : Game is ready !", "Cyan");
    }

    public void Register()
    {
        ServiceLocator.Register<IGameManagerReadService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IGameManagerReadService>(this);
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }

}
