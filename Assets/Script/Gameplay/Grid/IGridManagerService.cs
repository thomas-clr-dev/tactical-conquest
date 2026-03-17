using System;
using UnityEngine;

public interface IGridManagerService
{
    public void GenerateGrid();

    public event Action OnGridGenerated;
}
