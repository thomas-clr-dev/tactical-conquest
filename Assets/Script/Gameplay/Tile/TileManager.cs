using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TileManager : MonoBehaviour, IServiceMB, ITileManagerService
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tilesContainer;

    [SerializeField] private Material _sandyMat;
    [SerializeField] private Material _player1Mat;
    [SerializeField] private Material _player1BaseMat;
    [SerializeField] private Material _player2Mat;
    [SerializeField] private Material _player2BaseMat;
    [SerializeField] private Material _movementHighlightMat;

    private ITurnManagerService _iTurnManagerService;
    private IGridManagerService _iGridManagerService;

    private Dictionary<TileView, Vector3> m_TileDictionary = new Dictionary<TileView, Vector3>();

    private List<UnitSelectionData> _unitSelections = new List<UnitSelectionData>();

    public event Action<int, Vector3> OnBaseGenerated;

    private List<TileView> _highlightedTiles = new List<TileView>();
    private TileView _selectedTile = null;
    private UnitView _selectedUnit = null;

    private TileView _player1Base;
    private TileView _player2Base;

    public event Action<List<UnitSelectionData>, TileView> OnUnitSelectionRequested;
    public event Action OnUnitSelectionCancelled;

    public IReadOnlyList<UnitSelectionData> CurrentUnitSelections => _unitSelections.AsReadOnly();
    public TileView CurrentSelectedTile => _selectedTile;

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
            _iTurnManagerService.OnTurnChanged += ClearHighlight;
        }

        _iGridManagerService = ServiceLocator.Get<IGridManagerService>();
        if (_iGridManagerService != null)
        {
            _iGridManagerService.OnGridGenerated += ChangeTileVisibility;
        }
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;

        _iTurnManagerService.OnTurnChanged -= ChangeTileVisibility;
        _iTurnManagerService.OnTurnChanged -= ClearHighlight;
        _iGridManagerService.OnGridGenerated -= ChangeTileVisibility;

        foreach (var tile in m_TileDictionary.Keys)
        {
            if (tile != null)
            {
                tile.OnTileLeftClicked -= HandleTileLeftClick;
                tile.OnTileRightClicked -= HandleTileRightClick;
            }
        }

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

        newTile.layer = LayerMask.NameToLayer("Tiles");

        if (newTile.GetComponent<Collider>() == null)
        {
            Utils.ErrorLog($"Tile {newTile.name} has NO COLLIDER! Clicks won't work!");
        }

        if (newTile.GetComponent<TileView>())
        {
            TileView tileView = newTile.GetComponent<TileView>();
            m_TileDictionary.Add(tileView, tilePosition);

            tileView.OnTileLeftClicked += HandleTileLeftClick;
            tileView.OnTileRightClicked += HandleTileRightClick;
        }
    }

    public int GetConqueredTileCount(int currentPlayer)
    {
        return m_TileDictionary.Keys.Count(tile => tile.IsOwnedBy(currentPlayer));
    }

    private void HandleTileLeftClick(TileView tile, int mouseButton)
    {
        int currentPlayer = _iTurnManagerService.CurrentPlayerID;

        Utils.ColorLog($"Left click on {tile.name} (Owner : {tile.Owner})", "Purple");

        if (_highlightedTiles.Contains(tile) && _selectedTile != null)
        {
            Utils.ColorLog($"Moving unit to {tile.name}", "Green");

            if (_unitSelections.Count == 0 || _unitSelections.All(s => s.SelectedToMove == 0))
            {
                StartCoroutine(MoveUnitAlongPath(_selectedUnit, _selectedTile, tile));
            }
            else
            {
                ConfirmUnitSelection(tile);
            }
            return;
        }

        if (tile.IsOwnedBy(currentPlayer))
        {
            int unitCount = tile.GetUnitCount();

            if (unitCount > 0)
            {
                Utils.ColorLog($"{unitCount} unit(s) found on {tile.name}", "DarkGreen");

                var unitGroups = tile.GetUnitsGroupedByTypes();

                Utils.ColorLog("=== UNIT(S) ON TILE", "Yellow");
                foreach (var group in unitGroups)
                {
                    if (group.Value.Count > 0)
                    {
                        Utils.ColorLog($" {group.Key} : {group.Value.Count} unit(s)", "Cyan");
                    }
                }

                PrepareUnitSelection(tile, unitGroups);

                UnitView unit = tile.GetUnitOnTile();
                ShowMovementRange(tile, unit.UnitData.MaxMovement);
                _selectedTile = tile;
                _selectedUnit = unit;
            }
            else
            {
                Utils.ColorLog("No unit found on this tile", "Yellow");
                ClearHighlight();
            }
        }
        else
        {
            Utils.ColorLog($"Not Player {currentPlayer} tile", "Red");
            ClearHighlight();
        }
    }

    private void PrepareUnitSelection(TileView tile, Dictionary<UnitType, List<UnitView>> unitGroups)
    {
        _unitSelections.Clear();

        foreach (var group in unitGroups)
        {
            if (group.Value.Count > 0)
            {
                _unitSelections.Add(new UnitSelectionData(group.Key, group.Value.Count));
            }
        }

        Utils.ColorLog($"Prepared selection for {_unitSelections.Count} unit types");

        OnUnitSelectionRequested?.Invoke(_unitSelections, tile);
    }

    public void ConfirmUnitSelection(TileView destinationTile)
    {
        if (_selectedTile == null)
        {
            Utils.ErrorLog("No tile selected!");
            return;
        }

        // Récupérer les unités sélectionnées depuis _selectedTile
        List<UnitView> unitsToMove = GetSelectedUnits(_selectedTile);

        if (unitsToMove.Count == 0)
        {
            Utils.ErrorLog("No units selected to move!");
            return;
        }

        Utils.ColorLog($"Moving {unitsToMove.Count} unit(s) to {destinationTile.name}", "Green");

        // Déplacer chaque unité sélectionnée
        StartCoroutine(MoveMultipleUnits(unitsToMove, _selectedTile, destinationTile));
    }

    private List<UnitView> GetSelectedUnits(TileView tile)
    {
        List<UnitView> selectedUnits = new List<UnitView>();
        var unitGroups = tile.GetUnitsGroupedByTypes();

        foreach (var selection in _unitSelections)
        {
            if (selection.SelectedToMove > 0 && unitGroups.ContainsKey(selection.UnitType))
            {
                // Prendre le nombre d'unités sélectionnées de ce type
                List<UnitView> unitsOfType = unitGroups[selection.UnitType];
                int count = Mathf.Min(selection.SelectedToMove, unitsOfType.Count);

                for (int i = 0; i < count; i++)
                {
                    selectedUnits.Add(unitsOfType[i]);
                }
            }
        }

        return selectedUnits;
    }

    private IEnumerator MoveMultipleUnits(List<UnitView> units, TileView startTile, TileView endTile)
    {
        if (units.Count == 0)
        {
            Utils.ErrorLog("No units to move!");
            yield break;
        }

        List<Vector3> path = CalculatePath(startTile, endTile);

        if (path.Count == 0)
        {
            Utils.ErrorLog("No valid path found!");
            ClearHighlight();
            yield break;
        }

        Utils.ColorLog($"Moving {units.Count} unit(s) through {path.Count} positions", "Cyan");

        float moveSpeed = 5f;
        float unitSpacing = 0.3f;

        for (int i = 0; i < units.Count; i++)
        {
            UnitView unit = units[i];
            float delay = i * 0.1f;

            yield return new WaitForSeconds(delay);

            Vector3 offset = CalculateUnitOffset(i, units.Count, unitSpacing);
            StartCoroutine(MoveUnitAlongPathInternal(unit, path, moveSpeed, offset));
        }

        yield return new WaitForSeconds(path.Count / moveSpeed + 1f);

        int currentPlayer = _iTurnManagerService.CurrentPlayerID;
        if (currentPlayer == 1)
        {
            endTile.SetTile(_player1Mat, TileOwner.Player1);
        }
        else if (currentPlayer == 2)
        {
            endTile.SetTile(_player2Mat, TileOwner.Player2);
        }

        Utils.ColorLog("All units moved!", "Green");
        ClearHighlight();
    }

    private Vector3 CalculateUnitOffset(int index, int totalUnits, float spacing)
    {
        if (totalUnits == 1)
            return Vector3.zero;

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(totalUnits));
        int row = index / gridSize;
        int col = index % gridSize;

        float offsetX = (col - (gridSize - 1) / 2f) * spacing;
        float offsetZ = (row - (gridSize - 1) / 2f) * spacing;

        return new Vector3(offsetX, 0, offsetZ);
    }

    private IEnumerator MoveUnitAlongPathInternal(UnitView unit, List<Vector3> path, float moveSpeed, Vector3 finalOffset = default)
    {
        foreach (Vector3 targetPos in path)
        {
            Vector3 startPos = unit.transform.position;
            Vector3 endPos = new Vector3(targetPos.x, 0.2f, targetPos.z);

            float distance = Vector3.Distance(startPos, endPos);
            float duration = distance / moveSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                unit.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            unit.transform.position = endPos;
        }

        if (finalOffset != Vector3.zero)
        {
            unit.transform.position += finalOffset;
            Utils.ColorLog($"Unit positioned with offset: {finalOffset}", "Yellow");
        }
    }

    public void CancelUnitSelection()
    {
        Utils.ColorLog("Unit selection cancelled", "Yellow");
        ClearHighlight();
        OnUnitSelectionCancelled?.Invoke();
    }

    private IEnumerator MoveUnitAlongPath(UnitView unit, TileView startTile, TileView endTile)
    {
        List<Vector3> path = CalculatePath(startTile, endTile);

        if (path.Count == 0)
        {
            Utils.ErrorLog("No valid path found !");
            ClearHighlight();
            yield break;
        }

        Utils.ColorLog($"Moving through {path.Count} positions", "Cyan");

        float movSpeed = 5f;

        foreach (Vector3 targetPos in path)
        {
            Vector3 startPos = unit.transform.position;
            Vector3 endPos = new Vector3(targetPos.x, 0.2f, targetPos.z);

            float distance = Vector3.Distance(startPos, endPos);
            float duration = distance / movSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                unit.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return unit;
            }

            unit.transform.position = endPos;
        }

        Utils.ColorLog("Movement Complete !", "Green");

        int currentPlayer = _iTurnManagerService.CurrentPlayerID;

        if (currentPlayer == 1)
        {
            endTile.SetTile(_player1Mat, TileOwner.Player1);
        }
        else if (currentPlayer == 2)
        {
            endTile.SetTile(_player2Mat, TileOwner.Player2);
        }

        ClearHighlight();
    }

    private List<Vector3> CalculatePath(TileView startTile, TileView endTile)
    {
        List<Vector3> path = new List<Vector3>();

        if (!m_TileDictionary.ContainsKey(startTile) || !m_TileDictionary.ContainsKey(endTile))
        {
            return path;
        }

        Vector3 startPos = m_TileDictionary[startTile];
        Vector3 endPos = m_TileDictionary[endTile];
        float cellSize = _iGridManagerService.CellSize;

        int diffX = Mathf.RoundToInt((endPos.x - startPos.x) / cellSize);
        int diffZ = Mathf.RoundToInt((endPos.z - startPos.z) / cellSize);

        Vector3 currentPos = startPos;

        int stepX = diffX > 0 ? 1 : -1;
        for (int i = 0; i < Mathf.Abs(diffX); i++)
        {
            currentPos.x += stepX * cellSize;
            path.Add(currentPos);
        }

        int stepZ = diffZ > 0 ? 1 : -1;
        for (int i = 0; i < Mathf.Abs(diffZ); i++)
        {
            currentPos.z += stepZ * cellSize;
            path.Add(currentPos);
        }

        return path;
    }

    private void ShowMovementRange(TileView centerTile, int maxMovement)
    {
        ClearHighlight();

        Vector3 centerPos = centerTile.transform.position;

        foreach (var kvp in m_TileDictionary)
        {
            TileView tileKey = kvp.Key;
            Vector3 tilePos = kvp.Value;

            float distX = Mathf.Abs(tilePos.x - centerPos.x);
            float distZ = Mathf.Abs(tilePos.z - centerPos.z);
            float cellSize = _iGridManagerService.CellSize;

            int tileDistanceX = Mathf.RoundToInt(distX / cellSize);
            int tileDistanceZ = Mathf.RoundToInt(distZ / cellSize);
            int totalDistance = tileDistanceX + tileDistanceZ;

            if (totalDistance > 0 && totalDistance <= maxMovement)
            {
                if (tileKey.GetUnitOnTile() == null)
                {
                    tileKey.Highlight(_movementHighlightMat);
                    _highlightedTiles.Add(tileKey);
                }
            }
        }

        Utils.ColorLog($"Highlighted {_highlightedTiles.Count} tiles (Range: {maxMovement})", "Green");
    }

    private void ClearHighlight()
    {
        foreach (var tile in _highlightedTiles)
        {
            if (tile != null)
            {
                tile.RemoveHighlight();
            }
        }

        _highlightedTiles.Clear();
        _selectedTile = null;
        _selectedUnit = null;

        Utils.ColorLog("Cleared all highlighted tiles", "Yellow");
    }

    private void HandleTileRightClick(TileView tile, int mouseButton)
    {
        int currentPlayer = _iTurnManagerService.CurrentPlayerID;

        Utils.ColorLog($"Left click on {tile.name} (Owner : {tile.Owner})", "Purple");

        if (tile.IsOwnedBy(currentPlayer))
        {
            Utils.ColorLog($"Player {currentPlayer} tile - Opening troop purchase UI", "DarkGreen");
            //TODO Afficher les déplacement
        }
        else
        {
            Utils.ColorLog("Cannot buy troops on enemy tile", "Red");
        }
    }

    public void SubscribeToTileCheck(Action<TileView> onLeftClick, Action<TileView> onRightClick)
    {
        foreach (var tile in m_TileDictionary.Keys)
        {
            if (tile != null)
            {
                tile.OnTileLeftClicked += (t, button) => onLeftClick?.Invoke(t);
                tile.OnTileRightClicked += (t, button) => onRightClick?.Invoke(t);
            }
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
            whileFlag++;
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
            tilePlayer1.SetTile(_player1BaseMat, TileOwner.Player1);
            _player1Base = tilePlayer1;
            OnBaseGenerated?.Invoke((int)TileOwner.Player1, tilePlayer1.transform.position);
        }

        TileView tilePlayer2 = m_TileDictionary.FirstOrDefault(x => x.Value == basePlayer2).Key;
        if (tilePlayer2 != null)
        {
            tilePlayer2.SetTile(_player2BaseMat, TileOwner.Player2);
            _player2Base = tilePlayer2;
            OnBaseGenerated?.Invoke((int)TileOwner.Player2, tilePlayer2.transform.position);
        }

        foreach (TileView tile in m_TileDictionary.Keys)
        {
            TileOwner tileOwner = tile.GetTileOwner();
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
                    if (tile == _player1Base)
                    {
                        tile.SetTileVisibility(_player1BaseMat);
                    }
                    else
                    {
                        tile.SetTileVisibility(_player1Mat);
                    }
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
                    if (tile == _player2Base)
                    {
                        tile.SetTileVisibility(_player2BaseMat);
                    }
                    else
                    {
                        tile.SetTileVisibility(_player2Mat);
                    }
                }
                break;
        }
    }
}
