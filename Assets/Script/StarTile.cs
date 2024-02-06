using System;
using System.Collections.Generic;
using UnityEngine;

public class StarTile : Tile
{
    private int _range = 3;

    protected override Type GetTileViewType() => typeof(StarTileView);

    public override TileCallbackData OnTileDestroyed(Vector2Int tile, BoardData boardData)
    {
        int boardWidth = boardData.width;
        List<Vector2Int> deleteTiles = new List<Vector2Int>();

        //for (int x = 0; x < boardWidth; x++)
        //{
        //    Vector2Int pos = new Vector2Int(x, tile.y);
            
        //    if (x != tile.x)
        //        deleteTiles.Add(pos);
        //}

        for (int y = -_range; y <= _range; y++)
        {
            for (int x = -_range; x <= _range; x++)
            {
                Vector2Int point = new Vector2Int(tile.x + x, tile.y + y);
                if (point.x >= 0 && point.x < boardData.width && // x in board
                    point.y >= 0 && point.y < boardData.height && // y in board
                    ((point.x == tile.x && point.y > tile.y) || // up
                      point.y == tile.y || // left & right
                      Mathf.Abs(point.x - Mathf.Abs(tile.y - point.y)) == tile.x )) // diag down
                    deleteTiles.Add(point);
            }
        }

        return new TileCallbackData() { toDestroy = deleteTiles };
    }
}