using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopPanelUIManager : MonoBehaviour
{
    private Label _scoutLabel, _scoutPrice, _soldierLabel, _soldierPrice, _tankLabel, _tankPrice;
    private Button _scoutBuy, _soldierBuy, _tankBuy;
    private VisualElement _unitShopPanel;

    private ITurnManagerService _iTurnManagerService;
    private IEconomyManagerService _iEconomyManagerService;
    private ITileManagerService _iTileManagerService;
    private IUnitManagerService _iUnitManagerService;
    private void Awake()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();
        _iEconomyManagerService = ServiceLocator.Get<IEconomyManagerService>();
        _iTileManagerService = ServiceLocator.Get<ITileManagerService>();
        _iUnitManagerService = ServiceLocator.Get<IUnitManagerService>();

        if (_iTurnManagerService != null)
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _scoutLabel = root.Q<Label>("ScoutLabel");
            _scoutPrice = root.Q<Label>("ScoutPrice");
            root.Q<Button>("ScoutBuy").clicked += () => OnBuyScout();

            _soldierLabel = root.Q<Label>("SoldierLabel");
            _soldierPrice = root.Q<Label>("SoldierPrice");
            root.Q<Button>("SoldierBuy").clicked += () => OnBuySoldier();

            _tankLabel = root.Q<Label>("TankLabel");
            _tankPrice = root.Q<Label>("TankPrice");
            root.Q<Button>("TankBuy").clicked += () => OnBuyTank();

        }
        else
        {
            Utils.ErrorLog("ITurnManagerService not found in GameplayUIManager!");
        }
        //UpdateUI();
    }

    private void OnBuyScout()
    {
        throw new NotImplementedException();
    }

    private void OnBuySoldier()
    {
        throw new NotImplementedException();
    }

    private void OnBuyTank()
    {
        throw new NotImplementedException();
    }
}
