using UnityEngine;

public interface ITileManagerService
{
    public void CreateTile(int[,] tileCoordinate, float CellSize);
}
