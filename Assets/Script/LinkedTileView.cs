using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LinkedTileView : TileView
{
    private static readonly Dictionary<string,List<Vector2Int>> _linkedTiles = new Dictionary<string, List<Vector2Int>>();

    [SerializeField] private string linkKey = nameof(LinkedTileView);

    private List<Vector2Int> LinkedTiles
    {
        get
        {
            if (!_linkedTiles.TryGetValue(linkKey, out List<Vector2Int> list))
                _linkedTiles.Add(linkKey, new List<Vector2Int>());
            return _linkedTiles[linkKey];
        }
    }

    public override void OnTileCreated(Vector2Int tile, BoardView board)
    {
        LinkedTiles.Add(tile);
    }

    public override Tween OnTileDestroyed(Vector2Int tile, BoardView board)
    {
        LinkedTiles.Remove(tile);
        return base.OnTileDestroyed(tile, board);
    }

    public override Tween OnTileMove(MovedTileInfo moveInfo, BoardView board)
    {
        LinkedTiles.Add(moveInfo.to);
        LinkedTiles.Remove(moveInfo.from);
        return base.OnTileMove(moveInfo, board);
    }

    public override Tween OnTileSwap(Vector2Int tile, BoardSequence currentSequence, BoardView board)
    {
        currentSequence.ClearTiles(LinkedTiles);
        return base.OnTileSwap(tile, currentSequence, board);
    }
}
