using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class MaterialEntry
{
    public string Name;
    public Material Material;
}

public class GameManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] public Vector3Int GridSize = new Vector3Int(10, 0, 10);
    [SerializeField] public GameObject TilePrefab;
    [SerializeField] public Transform GridContainer;
    [Min(1f)][SerializeField] public float CellSize = 1.2f;

    [Header("Cameras Configuration")]
    [SerializeField] public List<Camera> CameraAvailable;

    [Header("Tile Configuration")]
    [SerializeField] public List<MaterialEntry> MaterialAvailable;

    private Dictionary<string, Material> _materialDict;

    private void Awake()
    {
        _materialDict = new Dictionary<string, Material>();
        foreach (var entry in MaterialAvailable)
        {
            if (!string.IsNullOrEmpty(entry.Name) && entry.Material != null)
            {
                _materialDict[entry.Name] = entry.Material;
            }
        }
    }

    private void Start()
    {
        Material DefaultMat;

        if (GetMaterialByName("Default") != null)
        {
            DefaultMat = GetMaterialByName("Default");
        }
        else
        {
            DefaultMat = MaterialAvailable[0].Material;
        }

        ServiceLocator.Get<IGridService>().GenerateGrid(GridSize, TilePrefab, GridContainer, CellSize, DefaultMat);
        ServiceLocator.Get<ICameraService>().SetCameraPositionFromGrid(GridSize, CellSize, CameraAvailable);
        ServiceLocator.Get<ICameraService>().RandomizeCameraSelection(CameraAvailable);
    }

    public Material GetMaterialByName(string name)
    {
        return _materialDict.TryGetValue(name, out Material mat) ? mat : null;
    }
}
