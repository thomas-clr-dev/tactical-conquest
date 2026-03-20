using UnityEngine;

public interface IPlayerService
{
    Color GetPlayerColor(int playerId);
    string GetPlayerName(int playerId);
}
