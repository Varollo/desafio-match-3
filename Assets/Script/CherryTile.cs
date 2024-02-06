using System;
using System.Collections.Generic;
using UnityEngine;

public class CherryTile : Tile
{
    private static readonly Dictionary<int, Dictionary<CherryTile, Vector2Int>> _linkedTiles = 
        new Dictionary<int, Dictionary<CherryTile, Vector2Int>>();

    protected override Type GetTileViewType() => typeof(CherryTileView);

    public override TileCallbackData OnTileCreated(Vector2Int tile, BoardData boardData)
    {
        if (!_linkedTiles.ContainsKey(group))
            _linkedTiles.Add(group, new Dictionary<CherryTile, Vector2Int>());
        _linkedTiles[group].Add(this, tile);

        return base.OnTileCreated(tile, boardData);
    }

    public override TileCallbackData OnTileDestroyed(Vector2Int tile, BoardData boardData)
    {
        List<Vector2Int> toDestroy = new List<Vector2Int>(_linkedTiles[group].Values);
        _linkedTiles[group].Clear();

        return new TileCallbackData() { toDestroy = toDestroy };
    }

    public override TileCallbackData OnTileMove(MovedTileInfo moveInfo, BoardData boardData)
    {
        _linkedTiles[group].Remove(this);
        _linkedTiles[group].Add(this,moveInfo.to);

        return base.OnTileMove(moveInfo, boardData);
    }
}
