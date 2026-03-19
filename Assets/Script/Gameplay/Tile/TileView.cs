using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileOwner
{
    Neutral = 0,
    Player1 = 1,
    Player2 = 2
}

public class TileView : MonoBehaviour
{
    public TileOwner Owner = TileOwner.Neutral;

    public event Action<TileView, int> OnTileLeftClicked;
    public event Action<TileView, int> OnTileRightClicked;

    private Renderer tileRenderer;
    private Material _originalMaterial;
    private bool _isHighlighted = false;

    private ITurnManagerService _iTurnManagerService;

    private void Awake()
    {

        _iTurnManagerService = ServiceLocator.Get<ITurnManagerService>();

        tileRenderer = gameObject.GetComponent<Renderer>();
        if (tileRenderer != null )
        {
            _originalMaterial = tileRenderer.material;
        }
    }

    public void TriggerLeftClick()
    {
        OnTileLeftClicked?.Invoke(this, 0);
    }

    public void TriggerRightClick()
    {
        OnTileRightClicked?.Invoke(this, 1);
    }

    public void Highlight(Material highlightMat)
    {
        if (tileRenderer != null && !_isHighlighted)
        {
            if ((int)Owner != _iTurnManagerService.CurrentPlayerID)
            {
                tileRenderer.material = highlightMat;
                _isHighlighted = true;
            }
            else
            {
                tileRenderer.material = _originalMaterial;
                _isHighlighted = true;
            }
        }
    }

    public void RemoveHighlight()
    {
        if (tileRenderer != null && _isHighlighted)
        {
            tileRenderer.material = _originalMaterial;
            _isHighlighted = false;
        }
    }

    public void SetTile(Material tileMat, TileOwner newTileOwner)
    {
        if (tileMat != null)
        {
            tileRenderer.material = tileMat;
            _originalMaterial = tileMat;
        }

        switch (newTileOwner)
        {
            case TileOwner.Neutral:
                Owner = newTileOwner; break;
            case TileOwner.Player1:
                Owner = newTileOwner; break;
            case TileOwner.Player2:
                Owner = newTileOwner; break;
            default:
                Owner = TileOwner.Neutral; break;
        }
    }

    public void SetTileVisibility(Material mat)
    {
        if (mat != null)
        {
            tileRenderer.material = mat;
            _originalMaterial = mat;
        }
    }

    public TileOwner GetTileOwner()
    {
        return Owner;
    }

    public int GetOwnerID()
    {
        return (int)Owner;
    }

    public bool IsOwnedBy(int playerID)
    {
        return (int)Owner == playerID;
    }

    public UnitView GetUnitOnTile()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Units"));

        foreach (var collider in colliders)
        {
            UnitView unit = collider.GetComponent<UnitView>();
            if (unit != null)
            {
                return unit; 
            }
        }

        return null;
    }

    public List<UnitView> GetAllUnitsOnTile()
    {
        List<UnitView> units = new List<UnitView>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Units"));

        foreach (var collider in colliders)
        {
            UnitView unit = collider.GetComponent<UnitView>();
            if (unit != null)
            {
                units.Add(unit); 
            }
        }

        return units;
    }

    public Dictionary<UnitType, List<UnitView>> GetUnitsGroupedByTypes()
    {
        var groups = new Dictionary<UnitType, List<UnitView>>()
        {
            { UnitType.Scout, new List<UnitView>() },
            { UnitType.Soldier, new List<UnitView>() },
            { UnitType.Tank, new List<UnitView>() }
        };

        List<UnitView> allUnits = GetAllUnitsOnTile();

        foreach (UnitView unit in allUnits)
        {
            UnitType type = unit.UnitData.UnitType;
            if (groups.ContainsKey(type))
            {
                groups[type].Add(unit);
            }
        }

        return groups;
    }

    public List<UnitView> GetUnitsByType(UnitType unitType)
    {
        return GetAllUnitsOnTile().Where(u => u.UnitData.UnitType == unitType).ToList();
    }

    public Dictionary<UnitType, int> GetUnitCountByType()
    {
        var counts = new Dictionary<UnitType, int>
        {
            { UnitType.Scout, 0 },
            { UnitType.Soldier, 0 },
            { UnitType.Tank, 0 }
        };

        foreach (UnitView unit in GetAllUnitsOnTile())
        {
            UnitType type = unit.UnitData.UnitType;
            if (counts.ContainsKey(type))
            {
                counts[type]++;
            }
        }

        return counts;
    }

    public bool HasMultipleUnits()
    {
        return GetAllUnitsOnTile().Count > 1;
    }

    public int GetUnitCount()
    {
        return GetAllUnitsOnTile().Count;
    }
}
