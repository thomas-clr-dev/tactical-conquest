using UnityEngine;

public class TileManager : MonoBehaviour, ITileService
{
    public Vector3Int tileCoordinate;

    public void SetTile(GameObject Tile,Material TileMat, Renderer TileRenderer)
    {
        TileRenderer.material = TileMat;
    }

    public Vector3Int GetTile()
    {
        return tileCoordinate;
    }
}
