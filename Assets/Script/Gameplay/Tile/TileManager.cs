using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour, IServiceMB, ITileManagerService
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tilesContainer;

    [SerializeField] private Material _sandyMat;
    [SerializeField] private Material _player1Mat;
    [SerializeField] private Material _player2Mat;

    private ITurnManagerService _iTurnManagerService;

    private Dictionary<TileView, Vector3> m_TileDictionary = new Dictionary<TileView, Vector3>();

    public event Action<int> OnBaseGenerated;

    private void Start()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();

        if (_iTurnManagerService != null)
        {
            _iTurnManagerService.OnTurnChanged += ChangeTileVisibility;
            ChangeTileVisibility();
        }
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }

    public void Register()
    {
        ServiceLocator.Register<ITileManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<ITileManagerService>(this);
    }

    public void CreateTile(int x, int y, float CellSize)
    {
        Vector3 tilePosition = new Vector3(x * CellSize, 0, y * CellSize);
        GameObject newTile = Instantiate(_tilePrefab, tilePosition, Quaternion.identity, _tilesContainer);

        newTile.name = $"Tile_{x}_{y}";

        if (newTile.GetComponent<TileView>())
        {
            m_TileDictionary.Add(newTile.GetComponent<TileView>(), tilePosition);
        }
    }

    public void SetPlayerBase(int gridLengthX, int gridLengthY, float cellSize)
    {
        int halfGridLengthX = Mathf.RoundToInt((gridLengthX * cellSize)) / 2;
        int halfGridLengthY = Mathf.RoundToInt((gridLengthY * cellSize)) / 2;

        int basePlayer1X = UnityEngine.Random.Range(0, gridLengthX);
        int basePlayer1Y = UnityEngine.Random.Range(0, gridLengthY);
        int basePlayer2X = UnityEngine.Random.Range(0, gridLengthX);
        int basePlayer2Y = UnityEngine.Random.Range(0, gridLengthY);

        int whileFlag = 0;
        int maxAttempt = 100;

        while (Mathf.Abs(basePlayer1X - basePlayer2X) < halfGridLengthX / cellSize && whileFlag < maxAttempt)
        {
            whileFlag ++;
            basePlayer1X = UnityEngine.Random.Range(0, gridLengthX);
        }

        whileFlag = 0;

        while (Mathf.Abs(basePlayer2Y - basePlayer1Y) < halfGridLengthY / cellSize && whileFlag < maxAttempt)
        {
            whileFlag++;
            basePlayer1Y = UnityEngine.Random.Range(0, gridLengthY);
        }

        Vector3 basePlayer1 = new Vector3(basePlayer1X * cellSize, 0, basePlayer1Y * cellSize);
        Vector3 basePlayer2 = new Vector3(basePlayer2X * cellSize, 0, basePlayer2Y * cellSize);

        TileView tilePlayer1 = m_TileDictionary.FirstOrDefault(x => x.Value == basePlayer1).Key;
        if (tilePlayer1 != null)
        {
            tilePlayer1.SetTile(_player1Mat, TileOwner.Player1);
            OnBaseGenerated?.Invoke(1);
        }
        else Utils.ErrorLog("Tile P1 not found !");

        TileView tilePlayer2 = m_TileDictionary.FirstOrDefault(x => x.Value == basePlayer2).Key;
        if (tilePlayer2 != null)
        {
            tilePlayer2.SetTile(_player2Mat, TileOwner.Player2);
            OnBaseGenerated?.Invoke(2);
        }
        else Utils.ErrorLog("Tile P2 not found !");

        foreach (TileView tile in m_TileDictionary.Keys)
        {
            TileOwner tileOwner = tile.GetTileOwner();
            Utils.ColorLog($"Owner : {tileOwner}", "Green");
        }
    }

    private void ChangeTileVisibility()
    {
        int currentPlayer = _iTurnManagerService.CurrentPlayerID;

        switch (_iTurnManagerService.CurrentPlayerID)
        {
            case 0:
                break;
            case 1:
                List<TileView> Notplayer1TilesList = m_TileDictionary.Keys.Where(tile => (int)tile.Owner != currentPlayer).ToList();
                foreach (TileView tile in Notplayer1TilesList)
                {
                    tile.SetTileVisibility(_sandyMat);
                }
                List<TileView> player1TilesList = m_TileDictionary.Keys.Where(tile => (int)tile.Owner == currentPlayer).ToList();
                foreach (TileView tile in player1TilesList)
                {
                    tile.SetTileVisibility(_player1Mat);
                }
                break;
            case 2:
                List<TileView> Notplayer2TilesList = m_TileDictionary.Keys.Where(tile => (int)tile.Owner != currentPlayer).ToList();
                foreach (TileView tile in Notplayer2TilesList)
                {
                    tile.SetTileVisibility(_sandyMat);
                }
                List<TileView> player2TilesList = m_TileDictionary.Keys.Where(tile => (int)tile.Owner == currentPlayer).ToList();
                foreach (TileView tile in player2TilesList)
                {
                    tile.SetTileVisibility(_player2Mat);
                }
                break;
        }
    }
}
