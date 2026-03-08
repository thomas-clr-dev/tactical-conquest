using UnityEngine;

public class GridManager : MonoBehaviour, IGridService
{
    public void GenerateGrid(Vector3Int GridSize, GameObject TilePrefab, Transform GridContainer, float CellSize, Material defaultMat)
    {
        if (GridContainer != null && TilePrefab != null)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.z; y++)
                {
                    Vector3 tilePosition = new Vector3(x * CellSize, 0, y * CellSize);
                    GameObject newTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity, GridContainer);
                    newTile.name = $"Tile_{x}_{y}";
                    Renderer newTileRenderer = newTile.GetComponent<Renderer>();
                    ServiceLocator.Get<ITileService>().SetTile(newTile, defaultMat, newTileRenderer);
                }
            }
            Utils.ColorLog("Grid generation success !", "Green");
            //temporary
            SetPLayersHC(GridSize);
        }
    }

    public void SetPLayersHC(Vector3Int GridSize)
    {

    }
}
