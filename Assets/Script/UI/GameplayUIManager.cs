using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIManager : MonoBehaviour, IServiceMB, IGameplayUIManagerService
{
    private Label _p1Name, _p1Turn, _p1Gold, _p1Conquest, _p1Forces;
    private VisualElement _p1Blind;

    private Label _p2Name, _p2Turn, _p2Gold, _p2Conquest, _p2Forces;
    private VisualElement _p2Blind;

    private ITurnManagerService _iTurnService;

    private void Awake()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        // ✅ Accéder aux services APRÈS qu'ils soient tous enregistrés
        _iTurnService = ServiceLocator.Get<ITurnManagerService>();

        if (_iTurnService != null)
        {
            Utils.ColorLog("GameplayUIManager: Services ready!", "Green");
            // Initialisation de l'UI ici

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

            _iTurnService.OnTurnChanged += UpdateUI;
            UpdateUI();
        }
        else
        {
            Utils.ErrorLog("ITurnManagerService not found in GameplayUIManager!");
        }
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
        if (_iTurnService.CurrentPlayerID == clickingPlayerID)
        {
            _iTurnService.EndTurn();
        }
    }

    private void UpdateUI()
    {
        int activePlayer = _iTurnService.CurrentPlayerID;
        int turn = _iTurnService.TurnNumber;
        //int gold = _iEconomService.GetGold(activePlayer);
        //int conquest = _iTileService.GetConquestedTile(activePlayer);
        //int forces = _iUnitService.GetUnitNumber(activePlayer);

        _p1Name.text = $"Player 1";
        _p1Turn.text = $"Turn : {turn}";
        //_p1Gold.text += $"{gold}";
        //_p1Conquest += $"{conquest}";
        //_p1Forces += $"{forces}";


        _p2Name.text = $"Player 2";
        _p2Turn.text = $"Turn : {turn}";
        //_p2Gold.text += $"{gold}";
        //_p2Conquest += $"{conquest}";
        //_p2Forces += $"{forces}";

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
    }

    private void OnQuitGameplay()
    {
        ServiceLocator.Get<IStartMenuUIManagerService>().OnQuitGameplay();
    }
}
