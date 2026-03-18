using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public interface IUnitManagerService
{
    List<UnitView> GetAllTroops(int playerID);

    public event Action OnUnitsGenerated;

    int GetTroopCount(int playerID);

}
