using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TileColorRepository colorRepository;

    protected virtual void OnDestroy()
    {
        transform.DOKill();
    }

    public virtual Tween OnTileCreated(BoardView board, Tile tile)
    {
        image.sprite = GetSprite(tile);
        image.color  = GetColor (tile, colorRepository);
        return null;
    }

    public virtual Tween OnTileDestroyed(BoardView board) => null;

    protected virtual Sprite GetSprite(Tile tile) => image.sprite;
    protected virtual Color GetColor(Tile tile, TileColorRepository colorRepository) => colorRepository.GetColor(tile.group);
}
