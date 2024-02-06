using System;
using UnityEngine;

public class Tile
{
    public int id;
    public int group;

    public TileView GetTileViewInstance(TilePrefabRepository repository) => repository.CreateTileInstance(GetTileViewType());

    protected virtual Type GetTileViewType() => typeof(TileView);
    public virtual TileCallbackData OnTileCreated(Vector2Int tile, BoardData boardData) => TileCallbackData.Empty;
    public virtual TileCallbackData OnTileSwap(Vector2Int tile, BoardData boardData) => TileCallbackData.Empty;
    public virtual TileCallbackData OnTileMove(MovedTileInfo moveInfo, BoardData boardData) => TileCallbackData.Empty;
    public virtual TileCallbackData OnTileDestroyed(Vector2Int tile, BoardData boardData) => TileCallbackData.Empty;
}
