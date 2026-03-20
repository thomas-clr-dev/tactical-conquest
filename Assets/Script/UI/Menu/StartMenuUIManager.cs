using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuUIManager : MonoBehaviour, IServiceMB, IStartMenuUIManagerService
{
    private GroupBox _menuRoot;
    private VisualElement _root;
    public bool IsGameRunning;

    public void Register()
    {
        ServiceLocator.Register<IStartMenuUIManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IStartMenuUIManagerService>(this);
    }

    private void Awake()
    {
        Register();

        IsGameRunning = false;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _menuRoot = _root.Q<GroupBox>("MenuBox");
        _menuRoot.Q<Button>("StartGameBtn").clicked += OnStartGame;
        _menuRoot.Q<Button>("QuitGameBtn").clicked += OnQuitGame;
    }

    private void OnStartGame()
    {
        if (IsGameRunning == false)
        {
            _root.style.display = DisplayStyle.None;
            IsGameRunning = true;
        }
        else
        {
            _root.style.display = DisplayStyle.Flex;
            IsGameRunning = false;
        }
    }

    private void OnQuitGame()
    {
        Utils.QuitGame();
    }

    public void OnQuitGameplay()
    {
        _root.style.display = DisplayStyle.Flex;
        IsGameRunning = !IsGameRunning;
        Utils.TogglePause();
    }
}
