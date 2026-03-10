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
    //[SerializeField] public List<MaterialEntry> MaterialAvailable;
    [SerializeField] public Material DefaultMat;
    [SerializeField] public Material Player1Mat;
    [SerializeField] public Material Player2Mat;
    [SerializeField] public Material FogMat;

    private Dictionary<string, Material> _materialDict;

    private void Start()
    {

        ServiceLocator.Get<IGridService>().GenerateGrid(GridSize, TilePrefab, GridContainer, CellSize, DefaultMat);
        ServiceLocator.Get<IGridService>().SetPlayersHC(GridSize, CellSize, Player1Mat, Player2Mat);
        ServiceLocator.Get<ICameraService>().SetCameraPositionFromGrid(GridSize, CellSize, CameraAvailable);
        ServiceLocator.Get<ICameraService>().RandomizeCameraSelection(CameraAvailable);
    }
}
