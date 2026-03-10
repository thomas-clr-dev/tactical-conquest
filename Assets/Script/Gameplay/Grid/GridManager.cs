using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour, IGridService
{
    private Dictionary<Vector3Int, GameObject> TileDictionary = new Dictionary<Vector3Int, GameObject>();
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

                    Vector3Int TileCoordinate = new Vector3Int(x, 0, y);
                    TileDictionary.Add(TileCoordinate, newTile);

                    ServiceLocator.Get<ITileService>().SetTile(defaultMat);
                }
            }
            Utils.ColorLog("Grid generation success !", "Green");
        }
    }

    public GameObject GetTileAt(Vector3Int coordinate)
    {
        if (TileDictionary.TryGetValue(coordinate, out GameObject tile))
        {
            return tile; 
        }
        return null;
    }

    public void SetPlayersHC(Vector3Int GridSize, float CellSize, Material Player1Mat, Material Player2Mat)
    {
        int MidXGridSize = Mathf.RoundToInt((GridSize.x * CellSize) / 2);
        int MidZGridSize = Mathf.RoundToInt((GridSize.z * CellSize) / 2);

        Vector3Int HCPlayer1 = new Vector3Int(Random.Range(0, GridSize.x), 0, Random.Range(0, GridSize.z));
        Vector3Int HCPlayer2 = new Vector3Int(Random.Range(0, GridSize.x), 0, Random.Range(0, GridSize.z));


        if (Mathf.Abs(HCPlayer1.x - HCPlayer2.x) < MidXGridSize)
        {
            while (Mathf.Abs(HCPlayer1.x - HCPlayer2.x) < MidXGridSize)
            {
                HCPlayer1.x = Random.Range(0, GridSize.x);
            }
        }
        else if (Mathf.Abs(HCPlayer1.z - HCPlayer2.z) < MidZGridSize)
        {
            while (Mathf.Abs(HCPlayer1.z - HCPlayer2.z) < MidZGridSize)
            {
                HCPlayer1.z = Random.Range(0, GridSize.z);
            }
        }

        //Utils.ColorLog($"Position HC P1 : {HCPlayer1}", "Cyan");
        //Utils.ColorLog($"Position HC P2 : {HCPlayer2}", "Red");

        //GameObject TileHCPlayer1 = GetTileAt(HCPlayer1);
        //Renderer Player1TileRenderer = TileHCPlayer1.GetComponent<Renderer>();
        //ServiceLocator.Get<ITileService>().SetTile(TileHCPlayer1, Player1Mat, Player1TileRenderer);

        //GameObject TileHCPlayer2 = GetTileAt(HCPlayer2);
        //Renderer Player2TileRenderer = TileHCPlayer2.GetComponent<Renderer>();
        //ServiceLocator.Get<ITileService>().SetTile(TileHCPlayer2 , Player2Mat, Player2TileRenderer);
    }
}
