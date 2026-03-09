using UnityEngine;

public class TileManager : MonoBehaviour, ITileService
{
    public Vector3Int tileCoordinate;
    public int OwnerID;

    public void SetTile(GameObject Tile,Material TileMat, Renderer TileRenderer)
    {
        TileRenderer.material = TileMat;
    }
}
