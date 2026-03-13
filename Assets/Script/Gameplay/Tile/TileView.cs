using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileOwner
{
    Neutral = 0,
    Player1 = 1,
    Player2 = 2
}

public class TileView : MonoBehaviour
{
    public TileOwner Owner = TileOwner.Neutral;

    private Renderer tileRenderer;

    private void Awake()
    {
        tileRenderer = gameObject.GetComponent<Renderer>();
    }

    public void SetTile(Material tileMat, TileOwner newTileOwner)
    {
        if (tileMat != null)
        {
            tileRenderer.material = tileMat;
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
}
