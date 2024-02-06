using DG.Tweening;
using UnityEngine;

public class StarTileView : TileView
{
    public override Tween OnTileDestroyed(BoardView board)
    {
        board.transform.DOKill(true);
        board.transform.DOPunchScale(Vector3.one * 0.1f, 1f).OnComplete(() => board.transform.DOScale(1f, .2f));
        return TweenUtils.GetBlankTween(.2f);
    }
}
