using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, IServiceMB, ITileManagerService
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tilesContainer;

    private Dictionary<TileView, Vector3> m_TileDictionary;

    private void Awake()
    {
        Register();
    }

    public void Register()
    {
        ServiceLocator.Register<ITileManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<ITileManagerService>(this);
    }

    public void CreateTile(int[,] tileCoordinate, float CellSize)
    {
        Vector3 tilePosition = new Vector3(tileCoordinate.GetLength(0) * CellSize, 0, tileCoordinate.GetLength(1) * CellSize);
        GameObject newTile = Instantiate(_tilePrefab, tilePosition, Quaternion.identity, _tilesContainer);

        newTile.name = $"Tile_{Mathf.RoundToInt(tilePosition.x)}_{Mathf.RoundToInt(tilePosition.z)}";

        if (newTile.GetComponent<TileView>())
        {
            m_TileDictionary.Add(newTile.GetComponent<TileView>(), tilePosition);
        }
    }
}
