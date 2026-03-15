using System;
using UnityEngine;

public class UnitManager : MonoBehaviour, IServiceMB, IUnitManagerService
{
    public void Register()
    {
        ServiceLocator.Register<IUnitManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IUnitManagerService>(this);
    }

    private void Awake()
    {
        Register();
    }

    private void Start()
    {
        ServiceLocator.Get<ITileManagerService>().OnBaseGenerated += OnTroopsGeneration;
    }

    private void OnTroopsGeneration()
    {

    }
}
