using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour, IServiceMB, IUnitManagerService
{
    private ITileManagerService _tileManagerService;
    private IPlayerService _playerService;
    private ITurnManagerService _iTurnManagerService;

    private List<UnitView> _allUnits = new List<UnitView>();

    private event Action _onUnitsGenerated;

    public Transform Player1TroopsContainer;
    public Transform Player2TroopsContainer;

    public UnitData ScoutData;
    public UnitData SoldierData;
    public UnitData TankData;

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
        _playerService = ServiceLocator.Get<IPlayerService>();

        if (_tileManagerService != null)
        {
            _tileManagerService.OnBaseGenerated += OnTroopsGeneration;
        }

        _onUnitsGenerated += ChangeUnitVisibility;

        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();

        if (_iTurnManagerService != null)
        {
            _iTurnManagerService.OnTurnChanged += ChangeUnitVisibility;
        }
    }

    private void ChangeUnitVisibility()
    {
        int currentPlayer = _iTurnManagerService.CurrentPlayerID;
        List<UnitView> allActualUnits = GetAllTroops();

        Utils.ColorLog($"Changing visibility for {allActualUnits.Count} units (Current Player: {currentPlayer})", "Yellow");

        foreach (var unitView in allActualUnits)
        {
            if (unitView != null)
            {
                bool isCurrentPlayer = unitView.IsOwnedBy(currentPlayer);
                Utils.SafeSetActive(unitView.gameObject, isCurrentPlayer);
            }
        }
    }

    public List<UnitView> GetAllTroops(int playerID)
    {
        return _allUnits.Where(unit => unit.IsOwnedBy(playerID)).ToList();
    }

    public List<UnitView> GetAllTroops()
    {
        return new List<UnitView>(_allUnits);
    }

    public int GetTroopCount(int playerID)
    {
        return _allUnits.Count(unit => unit.IsOwnedBy(playerID));
    }

    private void RemoveUnit(UnitView unitView)
    {
        _allUnits.Remove(unitView);
        Utils.ColorLog($"Unit removed from tracking : {unitView.gameObject.name}", "Yellow");
    }

    private void OnTroopsGeneration(int playerId, Vector3 basePosition)
    {
        Transform parentContainer = playerId == 1 ? Player1TroopsContainer : Player2TroopsContainer;

        Vector3 unitPosition = new Vector3(basePosition.x, basePosition.y + 0.2f, basePosition.z);

        GameObject unitGO = Instantiate(
            ScoutData.PrefabModel,
            unitPosition,
            Quaternion.identity,
            parentContainer
        );

        unitGO.layer = LayerMask.NameToLayer("Units");

        UnitView unitView = unitGO.GetComponent<UnitView>();
        if (unitView == null)
        {
            unitView = unitGO.AddComponent<UnitView>();
        }

        unitView.Initialize(playerId, ScoutData);

        if (_playerService != null)
        {
            Color playerColor = _playerService.GetPlayerColor(playerId);
            unitView.SetPlayerColor(playerColor);
        }

        _allUnits.Add(unitView);

        unitView.OnUnitDestroyed += RemoveUnit;

        Utils.ColorLog($"Unit added to tracking: {unitView.gameObject.name} (Total: {_allUnits.Count})", "Green");

        _onUnitsGenerated?.Invoke();
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;

        if (_tileManagerService != null)
        {
            _tileManagerService.OnBaseGenerated -= OnTroopsGeneration;
        }

        foreach (var unit in _allUnits)
        {
            if (unit != null)
            {
                unit.OnUnitDestroyed -= RemoveUnit;
            }
        }

        _allUnits.Clear();

        Unregister();
    }

}
