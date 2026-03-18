using UnityEngine;
using System.Collections.Generic;
using System;

public class GridManager : MonoBehaviour, IServiceMB, IGridManagerService
{
    [Header("Grid Configuration")]
    private int[,] _gridSize;
    [SerializeField] private float CellSize = 1.1f;
    [SerializeField] private int _gridLengthX = 10;
    [SerializeField] private int _gridLengthY = 10;

    float IGridManagerService.CellSize => CellSize;

    public event Action OnGridGenerated;

    private void Awake()
    {
        // Ne plus s'auto-register
        _gridSize = new int[_gridLengthX, _gridLengthY];
    }

    public void Register()
    {
        ServiceLocator.Register<IGridManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<IGridManagerService>(this);
    }

    public void GenerateGrid()
    {
        for (int x = 0; x < _gridSize.GetLength(0); x++)
        {
            for (int y = 0; y < _gridSize.GetLength(1); y++)
            {
                ServiceLocator.Get<ITileManagerService>().CreateTile(x, y, CellSize);
            }
        }
        ServiceLocator.Get<ITileManagerService>().SetPlayerBase(_gridLengthX, _gridLengthY, CellSize);
        ServiceLocator.Get<ICameraManagerService>().SetCameraPositionFromGrid(_gridLengthX, _gridLengthX, CellSize);
        OnGridGenerated?.Invoke();
    }
    private void OnDestroy()
    {
        Unregister();
    }
}
