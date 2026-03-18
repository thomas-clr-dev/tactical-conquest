using UnityEngine;
using System;

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

    private void Awake()
    {
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
            tileRenderer.material = highlightMat;
            _isHighlighted = true;
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
}
