using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IServiceMB, IGameManagerReadService
{
    private void Awake()
    {
        Register();
    }

    public void Register()
    {
        ServiceLocator.Register<IGameManagerReadService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IGameManagerReadService>(this);
    }

    private void Start()
    {
        ServiceLocator.Get<IGridManagerService>().GenerateGrid();
    }

}
