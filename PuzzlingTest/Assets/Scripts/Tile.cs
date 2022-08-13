using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer _sprite;
    bool _selected;

    private void Start()
    {
        _selected = false;
    }

    /// <summary>
    /// Making tile pressable
    /// </summary>
    private void OnMouseEnter()
    {
        if (_sprite != null)
        {
            _sprite.color = Color.cyan;
        }
    }

    private void OnMouseExit()
    {
        if (_sprite != null && !_selected)
        {
            _sprite.color = Color.white;
        }
        if(_selected)
        {
            _sprite.color = Color.gray;
        }
    }

    private void OnMouseDown()
    {
        if (_sprite != null && !_selected)
        {
            _sprite.color = Color.gray;
        }
        _selected = !_selected;
    }

}
