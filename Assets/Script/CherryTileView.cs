using DG.Tweening;

public class CherryTileView : TileView
{
    public override Tween OnTileDestroyed(BoardView board)
    {
        return transform.DOScale(0, .2f);
    }
}