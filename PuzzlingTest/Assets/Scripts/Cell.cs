using UnityEngine;

public class Cell : MonoBehaviour
{
    private Tile _tile;

    public Tile GetTile
    {
        get { return _tile; }
    }

    //Assigning a tile to every cell
    public void SetTile(Tile tile)
    {
        _tile = tile;
    }
}
