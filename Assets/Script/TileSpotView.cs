using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TileSpotView : MonoBehaviour
{
    [SerializeField] private Button _button;

    private int _x;
    private int _y;

    public event Action<int, int> onClick;

    private void Awake()
    {
        _button.onClick.AddListener(OnTileClick);
    }

    private void OnTileClick()
    {
        onClick?.Invoke(_x, _y);
    }

    public void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SetTile(TileView tile)
    {
        _button.targetGraphic = tile.GetComponent<Graphic>();
        tile.transform.SetParent(transform, false);
        tile.transform.position = transform.position;
    }

    public Tween AnimatedSetTile(TileView tile, float duration = 0.3f)
    {
        _button.targetGraphic = tile.GetComponent<Graphic>();
        tile.transform.SetParent(transform);
        return tile.transform.DOLocalMove(Vector3.zero, duration);
    }

}