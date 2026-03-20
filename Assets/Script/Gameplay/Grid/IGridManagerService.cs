using System;
using UnityEngine;

public interface IGridManagerService
{
    public float CellSize { get; }
    public void GenerateGrid();

    public event Action OnGridGenerated;
}
