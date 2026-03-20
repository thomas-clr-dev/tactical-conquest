using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public interface IUnitManagerService
{
    List<UnitView> GetAllTroops(int playerID);

    public event Action OnUnitsGenerated;

    public event Action<UnitData> OnTroopBought;

    int GetTroopCount(int playerID);

    void AddTroop(int playerId, TileView tile, UnitData unitData);

}
