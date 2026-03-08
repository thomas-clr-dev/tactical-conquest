using UnityEngine;

public interface IGridService
{
    public void GenerateGrid(Vector3Int GridSize, GameObject TilePrefab, Transform GridContainer, float CellSize, Material defaultMat);
}
