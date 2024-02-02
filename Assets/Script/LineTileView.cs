using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LineTileView : TileView
{
    public override Tween OnTileSwap(Vector2Int tile, BoardSequence currentSequence, BoardView board)
    {
        int boardWidth = board.GetWidth();
        for (int x = 0; x < boardWidth; x++)
        {
            Vector2Int pos = new Vector2Int(x, tile.y);
            
            if (!currentSequence.matchedPosition.Contains(pos))
                currentSequence.matchedPosition.Add(pos);
        }

        return base.OnTileSwap(tile, currentSequence, board);
    }
}