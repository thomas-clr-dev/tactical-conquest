using UnityEngine;

public interface IGridService
{
    public void GenerateGrid(Vector3Int GridSize, GameObject TilePrefab, Transform GridContainer, float CellSize, Material defaultMat);
    public GameObject GetTileAt(Vector3Int TileCoordinate);
    public void SetPlayersHC(Vector3Int GridSize, float CellSize, Material Player1Mat, Material Player2Mat);
}
