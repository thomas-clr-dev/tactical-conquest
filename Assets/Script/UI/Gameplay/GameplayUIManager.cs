using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIManager : MonoBehaviour, IServiceMB, IGameplayUIManagerService
{
    private Label _p1Name, _p1Turn, _p1Gold, _p1Conquest, _p1Forces;
    private VisualElement _p1Blind;

    private Label _p2Name, _p2Turn, _p2Gold, _p2Conquest, _p2Forces;
    private VisualElement _p2Blind;

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
        // ✅ Accéder aux services APRÈS qu'ils soient tous enregistrés
        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();
        _iEconomyManagerService = ServiceLocator.Get<IEconomyManagerService>();
        _iTileManagerService = ServiceLocator.Get<ITileManagerService>();
        _iUnitManagerService = ServiceLocator.Get<IUnitManagerService>();

        if (_iTurnManagerService != null)
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // --- SETUP Player 1 ---

            var p1Root = root.Q<VisualElement>("Player1Screen");
            _p1Blind = p1Root.Q<VisualElement>("Player1Overlay");
            _p1Name = p1Root.Q<Label>("Player1Name");
            _p1Turn = p1Root.Q<Label>("Player1Turn");
            _p1Gold = p1Root.Q<Label>("Player1Gold");
            _p1Conquest = p1Root.Q<Label>("Player1Conquest");
            _p1Forces = p1Root.Q<Label>("Player1Forces");
            p1Root.Q<Button>("EndTurnBtn").clicked += () => OnEndTurnClicked(1);
            p1Root.Q<Button>("QuitGameplayBtn").clicked += () => OnQuitGameplay();

            // --- SETUP Player 2 ---

            var p2Root = root.Q<VisualElement>("Player2Screen");
            _p2Blind = p2Root.Q<VisualElement>("Player2Overlay");
            _p2Name = p2Root.Q<Label>("Player2Name");
            _p2Turn = p2Root.Q<Label>("Player2Turn");
            _p2Gold = p2Root.Q<Label>("Player2Gold");
            _p2Conquest = p2Root.Q<Label>("Player2Conquest");
            _p2Forces = p2Root.Q<Label>("Player2Forces");
            p2Root.Q<Button>("EndTurnBtn").clicked += () => OnEndTurnClicked(2);
            p2Root.Q<Button>("QuitGameplayBtn").clicked += () => OnQuitGameplay();

            _iTurnManagerService.OnTurnChanged += UpdateUI;
            _iUnitManagerService.OnUnitsGenerated += UpdateUI;
        }
        else
        {
            Utils.ErrorLog("ITurnManagerService not found in GameplayUIManager!");
        }
        //UpdateUI();
    }

    public void Register()
    {
        ServiceLocator.Register<IGameplayUIManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IGameplayUIManagerService>(this);
    }

    private void OnEndTurnClicked(int clickingPlayerID)
    {
        if (_iTurnManagerService.CurrentPlayerID == clickingPlayerID)
        {
            _iTurnManagerService.EndTurn();
        }
    }

    private void UpdateUI()
    {
        int activePlayer = _iTurnManagerService.CurrentPlayerID;
        int turn = _iTurnManagerService.TurnNumber;

        int p1Gold = _iEconomyManagerService.GetPlayerGold(1);
        int p2Gold = _iEconomyManagerService.GetPlayerGold(2);

        int p1Conquest = _iTileManagerService?.GetConqueredTileCount(1) ?? 0;
        int p2Conquest = _iTileManagerService?.GetConqueredTileCount(2) ?? 0;

        int p1Forces = _iUnitManagerService?.GetTroopCount(1) ?? 0;
        int p2Forces = _iUnitManagerService?.GetTroopCount(2) ?? 0;

        _p1Name.text = "Player 1";
        _p1Turn.text = $"Turn : {turn}";
        _p1Gold.text = $"🪙 : {p1Gold}";
        _p1Conquest.text = $"🏴 : {p1Conquest}";
        _p1Forces.text = $"💪 : {p1Forces}";

        _p2Name.text = "Player 2";
        _p2Turn.text = $"Turn : {turn}";
        _p2Gold.text = $"🪙 : {p2Gold}";
        _p2Conquest.text = $"🏴 : {p2Conquest}";
        _p2Forces.text = $"💪 : {p2Forces}";

        if (activePlayer == 1)
        {
            _p1Blind.style.display = DisplayStyle.None;
            _p2Blind.style.display = DisplayStyle.Flex;
        }
        else if (activePlayer == 2)
        {
            _p2Blind.style.display = DisplayStyle.None;
            _p1Blind.style.display = DisplayStyle.Flex;
        }

        Utils.ColorLog("========== UI UPDATED !!!!! ==========", "Black");
        Utils.ColorLog($"Turn: {turn} | Active Player: {activePlayer}", "White");
        Utils.ColorLog($"P1 → Gold: {p1Gold} | Tiles: {p1Conquest} | Units: {p1Forces}", "Cyan");
        Utils.ColorLog($"P2 → Gold: {p2Gold} | Tiles: {p2Conquest} | Units: {p2Forces}", "Magenta");

    }

    private void OnQuitGameplay()
    {
        ServiceLocator.Get<IStartMenuUIManagerService>().OnQuitGameplay();
    }
}
