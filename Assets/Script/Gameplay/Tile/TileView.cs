using UnityEngine;

public class TileView : MonoBehaviour, ITileService
{
    public Vector3Int TileCoordinates;
    public int OwnerID = 0; // 0 = neutral, 1 = Player 1, 2 = Player 2

    private void Awake()
    {
        ServiceLocator.Register<ITileService>(this);
    }

    private void Start()
    {
        Renderer tileRenderer = gameObject.GetComponent<Renderer>();
    }

    public Vector3Int GetTile()
    {
        return TileCoordinates;
    }

    public void IsActualPlayerOwner(int ActualPlayer)
    {
        if (OwnerID == ActualPlayer)
        {
            
        }
    }

    public void SetTile(Material material)
    {
        throw new System.NotImplementedException();
    }
}
