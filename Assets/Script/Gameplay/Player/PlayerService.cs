using System;
using UnityEngine;

public class PlayerService : MonoBehaviour, IServiceMB, IPlayerService
{
    [SerializeField] private Color _player1Color = Color.cyan;
    [SerializeField] private Color _player2Color = new Color(0.5f, 0f, 0.5f); // Purple

    public void Register()
    {
        ServiceLocator.Register<IPlayerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IPlayerService>(this);
    }

    public Color GetPlayerColor(int playerId)
    {
        return playerId == 1 ? _player1Color : _player2Color;
    }

    public string GetPlayerName(int playerId)
    {
        return $"Player {playerId}";
    }

    private void OnDestroy()
    {
        Unregister();
    }
}
