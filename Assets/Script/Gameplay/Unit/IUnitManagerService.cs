using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IUnitManagerService
{
    List<UnitView> GetAllTroops(int playerID);

    int GetTroopCount(int playerID);

}
