using DG.Tweening;
using UnityEngine;

public class BombTileView : TileView
{
    public override Tween OnTileDestroyed(BoardView board)
    {
        board.DOKill(true);
        board.transform.DOShakePosition(.5f, 10f).OnComplete(() => board.transform.DOLocalMove(Vector3.zero, .2f));
        return base.OnTileDestroyed(board);
    }
}
