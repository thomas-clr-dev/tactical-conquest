using System;
using UnityEngine;

public interface ITileManagerService
{
    public void CreateTile(int x, int y, float CellSize);

    public void SetPlayerBase(int gridLengthX, int gridLengthY, float cellSize);

    public int GetConqueredTileCount(int currentPlayer);

    public event Action<int, Vector3> OnBaseGenerated;
}
